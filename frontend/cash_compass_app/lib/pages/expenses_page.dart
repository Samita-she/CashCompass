import 'dart:async';
import 'package:flutter/material.dart';
import '../api_service.dart';

// Define a simple structure for Category data used in the Dropdown
class CategoryItem {
  final int id;
  final String name;

  CategoryItem(this.id, this.name);
}

class ExpensesPage extends StatefulWidget {
  const ExpensesPage({super.key});

  @override
  State<ExpensesPage> createState() => _ExpensesPageState();
}

class _ExpensesPageState extends State<ExpensesPage> {
  Future<Map<String, dynamic>>? _pageData;
  List<CategoryItem> _categories = [];

  // State variable to hold the currently selected Category ID
  int? _selectedCategoryId;

  @override
  void initState() {
    super.initState();
    _loadPageData();
  }

  // Combine fetching both Expenses and Categories
  void _loadPageData() {
    _pageData =
        Future.wait([
          ApiService.fetchExpenses(),
          ApiService.fetchCategories(),
        ]).then((results) {
          final expenses = results[0] as List<dynamic>;
          final categoryList = results[1] as List<dynamic>;

          // Map API data to CategoryItem list
          _categories = categoryList.map((c) {
            return CategoryItem(
              c['categoryId'] ?? c['CategoryId'],
              c['categoryName'] ?? c['CategoryName'],
            );
          }).toList();

          return {'expenses': expenses};
        });
  }

  // Helper to show SnackBar feedback
  void _showSnackBar(String message, {bool isError = false}) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: isError ? Colors.red.shade700 : Colors.green.shade700,
        duration: const Duration(seconds: 2),
      ),
    );
  }

  // Helper function to show the date picker
  Future<void> _selectDate(TextEditingController controller) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime(2101),
    );
    if (picked != null) {
      // Format date as YYYY-MM-DD string
      controller.text = picked.toIso8601String().substring(0, 10);
    }
  }

  void _showExpenseDialog({Map<String, dynamic>? expense}) {
    // Initialize controllers
    final TextEditingController userIdController = TextEditingController(
      text: (expense?['UserId'] ?? expense?['userId'])?.toString() ?? '',
    );
    final TextEditingController nameController = TextEditingController(
      text: expense?['ExpenseName'] ?? expense?['expenseName'] ?? '',
    );
    final TextEditingController amountController = TextEditingController(
      text: (expense?['Amount'] ?? expense?['amount'])?.toString() ?? '',
    );
    final TextEditingController dateController = TextEditingController(
      text: expense?['ExpenseDate'] ?? expense?['expenseDate'] ?? '',
    );
    final TextEditingController notesController = TextEditingController(
      text: expense?['Notes'] ?? expense?['notes'] ?? '',
    );

    // Set initial selected category for editing
    _selectedCategoryId = expense?['CategoryId'] ?? expense?['categoryId'];

    // Get ID safely for update
    final int? expenseId = expense?['ExpenseId'] ?? expense?['expenseId'];

    // If categories are empty, show an error and return
    if (_categories.isEmpty) {
      _showSnackBar(
        'No categories available. Please add categories first.',
        isError: true,
      );
      return;
    }

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(expense == null ? 'Add Expense' : 'Edit Expense'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              // User ID (Required Field)
              TextField(
                controller: userIdController,
                decoration: const InputDecoration(labelText: 'User ID *'),
                keyboardType: TextInputType.number,
              ),

              // ⭐ CATEGORY DROPDOWN FIELD
              DropdownButtonFormField<int>(
                value: _selectedCategoryId,
                decoration: const InputDecoration(labelText: 'Category Name *'),
                items: _categories.map((CategoryItem cat) {
                  return DropdownMenuItem<int>(
                    value: cat.id,
                    child: Text(cat.name),
                  );
                }).toList(),
                onChanged: (int? newValue) {
                  setState(() {
                    _selectedCategoryId = newValue;
                  });
                },
                validator: (value) =>
                    value == null ? 'Category is required' : null,
              ),
              const SizedBox(height: 12), // Spacing after dropdown
              // Remaining Fields
              TextField(
                controller: nameController,
                decoration: const InputDecoration(labelText: 'Expense Name *'),
              ),
              TextField(
                controller: amountController,
                decoration: const InputDecoration(labelText: 'Amount *'),
                keyboardType: TextInputType.number,
              ),
              TextField(
                controller: dateController,
                decoration: InputDecoration(
                  labelText: 'Expense Date (YYYY-MM-DD) *',
                  suffixIcon: IconButton(
                    icon: const Icon(Icons.calendar_today),
                    onPressed: () => _selectDate(dateController),
                  ),
                ),
                readOnly: true,
              ),
              TextField(
                controller: notesController,
                decoration: const InputDecoration(labelText: 'Notes'),
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () async {
              final parsedUserId = int.tryParse(userIdController.text);
              final parsedAmount = double.tryParse(amountController.text);

              if (parsedUserId == null ||
                  _selectedCategoryId == null || // Check selected Category ID
                  nameController.text.isEmpty ||
                  parsedAmount == null ||
                  parsedAmount <= 0 ||
                  dateController.text.isEmpty) {
                _showSnackBar(
                  'Please fill in all required fields (marked *) with valid data.',
                  isError: true,
                );
                return;
              }

              final data = {
                'CategoryId': _selectedCategoryId, // Use the selected ID
                'UserId': parsedUserId,
                'ExpenseName': nameController.text,
                'Amount': parsedAmount,
                'ExpenseDate': dateController.text,
                'Notes': notesController.text,
              };

              bool success = false;

              try {
                if (expense == null) {
                  await ApiService.addExpense(data);
                  _showSnackBar('Expense added successfully!');
                  success = true;
                } else {
                  if (expenseId != null) {
                    await ApiService.updateExpense(expenseId, data);
                    _showSnackBar('Expense updated successfully!');
                    success = true;
                  }
                }
              } catch (e) {
                _showSnackBar(
                  'Failed to save expense: ${e.toString()}',
                  isError: true,
                );
              }

              // ⭐ Stability Fix: Only pop and reload IF successful
              if (success) {
                if (mounted) Navigator.pop(context);
                setState(() => _loadPageData());
              }
              // Reset selection variable after potential state update
              _selectedCategoryId = null;
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteExpense(int id) async {
    bool success = false;
    try {
      await ApiService.deleteExpense(id);
      _showSnackBar('Expense deleted successfully!');
      success = true;
    } catch (e) {
      // ⭐ Stability Fix: Show SnackBar on failure
      _showSnackBar('Failed to delete expense: ${e.toString()}', isError: true);
    }

    // ⭐ Stability Fix: Only reload the state if the deletion was successful
    if (success) {
      setState(() => _loadPageData());
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Expenses')),
      body: FutureBuilder<Map<String, dynamic>>(
        future: _pageData,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Text(
                  'Error fetching data. Check server console and API URL.\n\nDetails: ${snapshot.error.toString()}',
                  textAlign: TextAlign.center,
                ),
              ),
            );
          } else if (!snapshot.hasData ||
              (snapshot.data!['expenses'] as List).isEmpty) {
            return const Center(
              child: Text('No expenses found. Tap + to add one.'),
            );
          } else {
            final expenses = snapshot.data!['expenses'] as List<dynamic>;
            return ListView.builder(
              itemCount: expenses.length,
              itemBuilder: (context, index) {
                final exp = expenses[index];

                // Safely extract data
                final name =
                    exp['ExpenseName'] ??
                    exp['expenseName'] ??
                    'Unnamed Expense';
                final amount = (exp['Amount'] ?? exp['amount'] ?? 0.0)
                    .toString();
                final date = exp['ExpenseDate'] ?? exp['expenseDate'] ?? 'N/A';
                final id = exp['ExpenseId'] ?? exp['expenseId'];
                final categoryId = exp['CategoryId'] ?? exp['categoryId'];

                // Find Category Name for display
                final categoryName = _categories
                    .firstWhere(
                      (cat) => cat.id == categoryId,
                      orElse: () =>
                          CategoryItem(categoryId, 'Unknown Category'),
                    )
                    .name;

                return ListTile(
                  title: Text(name),
                  subtitle: Text(
                    'Amount: \$${amount} | Date: ${date.substring(0, 10)} | Category: ${categoryName}',
                  ),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: const Icon(Icons.edit),
                        onPressed: () => _showExpenseDialog(expense: exp),
                      ),
                      IconButton(
                        icon: const Icon(Icons.delete),
                        onPressed: () {
                          if (id != null) {
                            _deleteExpense(id);
                          }
                        },
                      ),
                    ],
                  ),
                );
              },
            );
          }
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _showExpenseDialog(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
