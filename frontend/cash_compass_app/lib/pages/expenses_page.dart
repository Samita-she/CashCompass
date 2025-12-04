import 'dart:async';
import 'package:flutter/material.dart';
import '../api_service.dart';

class ExpensesPage extends StatefulWidget {
  const ExpensesPage({super.key});

  @override
  State<ExpensesPage> createState() => _ExpensesPageState();
}

class _ExpensesPageState extends State<ExpensesPage> {
  Future<List<dynamic>>? _expenses;

  @override
  void initState() {
    super.initState();
    _loadExpenses();
  }

  void _loadExpenses() {
    _expenses = ApiService.fetchExpenses();
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
    // Controllers initialized with existing data (PascalCase/camelCase safe)
    final TextEditingController categoryIdController = TextEditingController(
      text:
          (expense?['CategoryId'] ?? expense?['categoryId'])?.toString() ?? '',
    );
    final TextEditingController userIdController = TextEditingController(
      text: (expense?['UserId'] ?? expense?['userId'])?.toString() ?? '',
    );
    final TextEditingController nameController = TextEditingController(
      text: expense?['ExpenseName'] ?? expense?['expenseName'] ?? '',
    );
    final TextEditingController amountController = TextEditingController(
      text: (expense?['Amount'] ?? expense?['amount'])?.toString() ?? '',
    );
    // Date Picker UX Fix: Controller for the date
    final TextEditingController dateController = TextEditingController(
      text: expense?['ExpenseDate'] ?? expense?['expenseDate'] ?? '',
    );
    final TextEditingController notesController = TextEditingController(
      text: expense?['Notes'] ?? expense?['notes'] ?? '',
    );

    // Get ID safely for update
    final int? expenseId = expense?['ExpenseId'] ?? expense?['expenseId'];

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(expense == null ? 'Add Expense' : 'Edit Expense'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: userIdController,
                decoration: const InputDecoration(labelText: 'User ID *'),
                keyboardType: TextInputType.number,
              ),
              TextField(
                controller: categoryIdController,
                decoration: const InputDecoration(labelText: 'Category ID *'),
                keyboardType: TextInputType.number,
              ),
              TextField(
                controller: nameController,
                decoration: const InputDecoration(labelText: 'Expense Name *'),
              ),
              TextField(
                controller: amountController,
                decoration: const InputDecoration(labelText: 'Amount *'),
                keyboardType: TextInputType.number,
              ),
              // Use a suffix icon to trigger the picker
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
              final parsedCategoryId = int.tryParse(categoryIdController.text);
              final parsedAmount = double.tryParse(amountController.text);

              // Validation Fix: Check required fields
              if (parsedUserId == null ||
                  parsedCategoryId == null ||
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

              // Ensure keys sent to the C# API are PascalCase
              final data = {
                'CategoryId': parsedCategoryId,
                'UserId': parsedUserId,
                'ExpenseName': nameController.text,
                'Amount': parsedAmount,
                'ExpenseDate': dateController.text,
                'Notes': notesController.text,
              };

              try {
                if (expense == null) {
                  await ApiService.addExpense(data);
                  _showSnackBar('Expense added successfully!');
                } else {
                  if (expenseId != null) {
                    await ApiService.updateExpense(expenseId, data);
                    _showSnackBar('Expense updated successfully!');
                  }
                }
              } catch (e) {
                // Error Handling Fix: Show SnackBar on failure
                _showSnackBar(
                  'Failed to save expense: ${e.toString()}',
                  isError: true,
                );
              }

              if (mounted) Navigator.pop(context);
              setState(() => _loadExpenses());
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteExpense(int id) async {
    try {
      await ApiService.deleteExpense(id);
      _showSnackBar('Expense deleted successfully!');
    } catch (e) {
      //Show SnackBar on failure
      _showSnackBar('Failed to delete expense: ${e.toString()}', isError: true);
    }
    setState(() => _loadExpenses());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Expenses (REST API)')),
      body: FutureBuilder<List<dynamic>>(
        future: _expenses,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            // Display friendly error message
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Text(
                  'Error fetching data. Check server console and API URL.\n\nDetails: ${snapshot.error.toString()}',
                  textAlign: TextAlign.center,
                ),
              ),
            );
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(
              child: Text('No expenses found. Tap + to add one.'),
            );
          } else {
            final expenses = snapshot.data!;
            return ListView.builder(
              itemCount: expenses.length,
              itemBuilder: (context, index) {
                final exp = expenses[index];

                // Safely extract and display data
                final name =
                    exp['ExpenseName'] ??
                    exp['expenseName'] ??
                    'Unnamed Expense';
                final amount = (exp['Amount'] ?? exp['amount'] ?? 0.0)
                    .toString();
                final date = exp['ExpenseDate'] ?? exp['expenseDate'] ?? 'N/A';
                final id = exp['ExpenseId'] ?? exp['expenseId'];

                return ListTile(
                  title: Text(name),

                  subtitle: Text(
                    'Amount: \$${amount} | Date: ${date.substring(0, 10)} | Cat ID: ${exp['CategoryId'] ?? exp['categoryId']}',
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
