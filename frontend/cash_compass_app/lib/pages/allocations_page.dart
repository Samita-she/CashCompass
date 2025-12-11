import 'dart:async';
import 'package:flutter/material.dart';
import '../api_service.dart';

// Reusing CategoryItem structure
class Item {
  final int id;
  final String name;

  Item(this.id, this.name);
}

class AllocationsPage extends StatefulWidget {
  const AllocationsPage({super.key});

  @override
  State<AllocationsPage> createState() => _AllocationsPageState();
}

class _AllocationsPageState extends State<AllocationsPage> {
  // Future now holds all page data (allocations, categories, income sources)
  Future<Map<String, dynamic>>? _pageData;

  List<Item> _categories = [];
  List<Item> _incomeSources = [];

  // State variables to hold the currently selected IDs
  int? _selectedCategoryId;
  int? _selectedIncomeId;
  String? _selectedAllocationType;

  // List of valid Allocation Types for a dropdown if needed (for now, using text field)
  final List<String> _allocationTypes = ['Fixed', 'Percent', 'Remaining'];

  @override
  void initState() {
    super.initState();
    _loadPageData();
  }

  // Combine fetching all necessary data
  void _loadPageData() {
    _pageData =
        Future.wait([
          ApiService.fetchAllocations(),
          ApiService.fetchCategories(),
          ApiService.fetchIncomeSources(),
        ]).then((results) {
          final allocations = results[0] as List<dynamic>;
          final categoryList = results[1] as List<dynamic>;
          final incomeList = results[2] as List<dynamic>;

          // Map API data to Item list for Categories
          _categories = categoryList.map((c) {
            return Item(
              c['categoryId'] ?? c['CategoryId'],
              c['categoryName'] ?? c['CategoryName'],
            );
          }).toList();

          // Map API data to Item list for Income Sources
          _incomeSources = incomeList.map((i) {
            return Item(
              i['incomeId'] ?? i['IncomeId'],
              i['sourceName'] ?? i['SourceName'],
            );
          }).toList();

          return {'allocations': allocations};
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

  void _showAllocationDialog({Map<String, dynamic>? allocation}) {
    // Reset selections for new dialog, or set for edit
    _selectedCategoryId =
        allocation?['CategoryId'] ?? allocation?['categoryId'];
    _selectedIncomeId = allocation?['IncomeId'] ?? allocation?['incomeId'];
    _selectedAllocationType =
        allocation?['AllocationType'] ?? allocation?['allocationType'];

    final TextEditingController userIdController = TextEditingController(
      text: (allocation?['UserId'] ?? allocation?['userId'])?.toString() ?? '',
    );
    // Removed incomeIdController and categoryIdController
    final TextEditingController typeController = TextEditingController(
      text: _selectedAllocationType ?? '', // Use the state variable
    );
    final TextEditingController valueController = TextEditingController(
      text:
          (allocation?['AllocationValue'] ?? allocation?['allocationValue'])
              ?.toString() ??
          '',
    );

    // Check if necessary lists are empty before showing dialog
    if (_categories.isEmpty || _incomeSources.isEmpty) {
      _showSnackBar(
        'Cannot add/edit allocation. Please ensure categories and income sources exist.',
        isError: true,
      );
      return;
    }

    // Get ID safely for update
    final int? allocationId =
        allocation?['AllocationId'] ?? allocation?['allocationId'];

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(allocation == null ? 'Add Allocation' : 'Edit Allocation'),
        content: StatefulBuilder(
          // Use StatefulBuilder to manage dialog state (dropdowns)
          builder: (BuildContext context, StateSetter setState) {
            return SingleChildScrollView(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  TextField(
                    controller: userIdController,
                    decoration: const InputDecoration(labelText: 'User ID *'),
                    keyboardType: TextInputType.number,
                  ),

                  // ⭐ INCOME SOURCE DROPDOWN
                  DropdownButtonFormField<int>(
                    value: _selectedIncomeId,
                    decoration: const InputDecoration(
                      labelText: 'Income Source *',
                    ),
                    items: _incomeSources.map((Item item) {
                      return DropdownMenuItem<int>(
                        value: item.id,
                        child: Text(item.name),
                      );
                    }).toList(),
                    onChanged: (int? newValue) {
                      setState(() {
                        _selectedIncomeId = newValue;
                      });
                    },
                    validator: (value) =>
                        value == null ? 'Income Source is required' : null,
                  ),
                  const SizedBox(height: 12),

                  // ⭐ CATEGORY DROPDOWN
                  DropdownButtonFormField<int>(
                    value: _selectedCategoryId,
                    decoration: const InputDecoration(
                      labelText: 'Category Name *',
                    ),
                    items: _categories.map((Item item) {
                      return DropdownMenuItem<int>(
                        value: item.id,
                        child: Text(item.name),
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
                  const SizedBox(height: 12),

                  // Allocation Type Field (could also be a dropdown using _allocationTypes)
                  TextField(
                    controller: typeController,
                    decoration: const InputDecoration(
                      labelText: 'Allocation Type * (e.g., Fixed, Percent)',
                    ),
                  ),
                  TextField(
                    controller: valueController,
                    decoration: const InputDecoration(
                      labelText: 'Allocation Value *',
                    ),
                    keyboardType: TextInputType.number,
                  ),
                ],
              ),
            );
          },
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () async {
              // Now use the state variables for IDs
              final parsedUserId = int.tryParse(userIdController.text);
              final parsedValue = double.tryParse(valueController.text);

              // FIX 2: Consolidate and correct validation block
              if (parsedUserId == null ||
                  _selectedIncomeId == null ||
                  _selectedCategoryId == null ||
                  typeController.text.isEmpty ||
                  parsedValue == null ||
                  parsedValue < 0) {
                _showSnackBar(
                  'Please fill in all required fields (*) with valid data.',
                  isError: true,
                );
                return;
              }

              // Data construction uses selected IDs
              final data = {
                'UserId': parsedUserId,
                'IncomeId': _selectedIncomeId, // Use selected ID
                'CategoryId': _selectedCategoryId, // Use selected ID
                'AllocationType': typeController.text,
                'AllocationValue': parsedValue,
              };

              bool success = false;
              try {
                if (allocation == null) {
                  await ApiService.addAllocation(data);
                  _showSnackBar('Allocation added successfully!');
                  success = true;
                } else {
                  if (allocationId != null) {
                    await ApiService.updateAllocation(allocationId, data);
                    _showSnackBar('Allocation updated successfully!');
                    success = true;
                  }
                }
              } catch (e) {
                // Error Handling Fix: Show SnackBar on failure
                _showSnackBar(
                  'Failed to save allocation: ${e.toString()}',
                  isError: true,
                );
              }

              // ⭐ Stability Fix: Only pop and reload IF successful
              if (success) {
                if (mounted) Navigator.pop(context);
                setState(() => _loadPageData());
              }
              // Reset selection variables after save attempt
              _selectedCategoryId = null;
              _selectedIncomeId = null;
              _selectedAllocationType = null;
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteAllocation(int id) async {
    bool success = false;
    try {
      await ApiService.deleteAllocation(id);
      _showSnackBar('Allocation deleted successfully!');
      success = true;
    } catch (e) {
      // Error Handling Fix: Show SnackBar on failure
      _showSnackBar(
        'Failed to delete allocation: ${e.toString()}',
        isError: true,
      );
    }

    // ⭐ Stability Fix: Only reload the state if the deletion was successful
    if (success) {
      setState(() => _loadPageData());
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Allocations')),
      body: FutureBuilder<Map<String, dynamic>>(
        future: _pageData,
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
          } else if (!snapshot.hasData ||
              (snapshot.data!['allocations'] as List).isEmpty) {
            return const Center(
              child: Text('No allocations found. Tap + to add one.'),
            );
          } else {
            final allocations = snapshot.data!['allocations'] as List<dynamic>;
            return ListView.builder(
              itemCount: allocations.length,
              itemBuilder: (context, index) {
                final alloc = allocations[index];

                // Safely extract data
                final id = alloc['AllocationId'] ?? alloc['allocationId'];
                final type =
                    alloc['AllocationType'] ?? alloc['allocationType'] ?? 'N/A';
                final value =
                    (alloc['AllocationValue'] ??
                            alloc['allocationValue'] ??
                            0.0)
                        .toString();
                final incomeId = alloc['IncomeId'] ?? alloc['incomeId'];
                final categoryId = alloc['CategoryId'] ?? alloc['categoryId'];

                // ⭐ Find Names for display
                final incomeName = _incomeSources
                    .firstWhere(
                      (item) => item.id == incomeId,
                      orElse: () => Item(incomeId, 'Unknown Income'),
                    )
                    .name;
                final categoryName = _categories
                    .firstWhere(
                      (item) => item.id == categoryId,
                      orElse: () => Item(categoryId, 'Unknown Category'),
                    )
                    .name;

                return ListTile(
                  title: Text('ID: $id | Type: $type'),
                  // ⭐ Display Names instead of IDs
                  subtitle: Text(
                    'Value: \$${value} | Income: $incomeName | Category: $categoryName',
                  ),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: const Icon(Icons.edit),
                        onPressed: () =>
                            _showAllocationDialog(allocation: alloc),
                      ),
                      IconButton(
                        icon: const Icon(Icons.delete),
                        onPressed: () {
                          if (id != null) {
                            _deleteAllocation(id);
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
        onPressed: () => _showAllocationDialog(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
