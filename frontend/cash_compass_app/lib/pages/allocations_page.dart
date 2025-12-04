import 'dart:async';
import 'package:flutter/material.dart';
import '../api_service.dart';

class AllocationsPage extends StatefulWidget {
  const AllocationsPage({super.key});

  @override
  State<AllocationsPage> createState() => _AllocationsPageState();
}

class _AllocationsPageState extends State<AllocationsPage> {
  Future<List<dynamic>>? _allocations;

  @override
  void initState() {
    super.initState();
    _loadAllocations();
  }

  void _loadAllocations() {
    _allocations = ApiService.fetchAllocations();
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
    // Controllers initialized with existing data (PascalCase/camelCase safe)
    final TextEditingController incomeIdController = TextEditingController(
      text:
          (allocation?['IncomeId'] ?? allocation?['incomeId'])?.toString() ??
          '',
    );
    final TextEditingController categoryIdController = TextEditingController(
      text:
          (allocation?['CategoryId'] ?? allocation?['categoryId'])
              ?.toString() ??
          '',
    );
    final TextEditingController typeController = TextEditingController(
      text:
          allocation?['AllocationType'] ?? allocation?['allocationType'] ?? '',
    );
    final TextEditingController valueController = TextEditingController(
      text:
          (allocation?['AllocationValue'] ?? allocation?['allocationValue'])
              ?.toString() ??
          '',
    );

    // Get ID safely for update
    final int? allocationId =
        allocation?['AllocationId'] ?? allocation?['allocationId'];

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(allocation == null ? 'Add Allocation' : 'Edit Allocation'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: incomeIdController,
                decoration: const InputDecoration(labelText: 'Income ID *'),
                keyboardType: TextInputType.number,
              ),
              TextField(
                controller: categoryIdController,
                decoration: const InputDecoration(labelText: 'Category ID *'),
                keyboardType: TextInputType.number,
              ),
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
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () async {
              final parsedIncomeId = int.tryParse(incomeIdController.text);
              final parsedCategoryId = int.tryParse(categoryIdController.text);
              final parsedValue = double.tryParse(valueController.text);

              if (parsedIncomeId == null ||
                  parsedCategoryId == null ||
                  typeController.text.isEmpty ||
                  parsedValue == null ||
                  parsedValue < 0) {
                _showSnackBar(
                  'Please fill in all required fields (*) with valid numbers/text.',
                  isError: true,
                );
                return;
              }

              final data = {
                'IncomeId': parsedIncomeId,
                'CategoryId': parsedCategoryId,
                'AllocationType': typeController.text,
                'AllocationValue': parsedValue,
              };

              try {
                if (allocation == null) {
                  await ApiService.addAllocation(data);
                  _showSnackBar('Allocation added successfully!');
                } else {
                  if (allocationId != null) {
                    await ApiService.updateAllocation(allocationId, data);
                    _showSnackBar('Allocation updated successfully!');
                  }
                }
              } catch (e) {
                // Error Handling Fix: Show SnackBar on failure
                _showSnackBar(
                  'Failed to save allocation: ${e.toString()}',
                  isError: true,
                );
              }

              if (mounted) Navigator.pop(context);
              setState(() => _loadAllocations());
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteAllocation(int id) async {
    try {
      await ApiService.deleteAllocation(id);
      _showSnackBar('Allocation deleted successfully!');
    } catch (e) {
      // Error Handling Fix: Show SnackBar on failure
      _showSnackBar(
        'Failed to delete allocation: ${e.toString()}',
        isError: true,
      );
    }
    setState(() => _loadAllocations());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Allocations (REST API)')),
      body: FutureBuilder<List<dynamic>>(
        future: _allocations,
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
              child: Text('No allocations found. Tap + to add one.'),
            );
          } else {
            final allocations = snapshot.data!;
            return ListView.builder(
              itemCount: allocations.length,
              itemBuilder: (context, index) {
                final alloc = allocations[index];

                // Safely extract and display data
                final id = alloc['AllocationId'] ?? alloc['allocationId'];
                final type =
                    alloc['AllocationType'] ?? alloc['allocationType'] ?? 'N/A';
                final value =
                    (alloc['AllocationValue'] ??
                            alloc['allocationValue'] ??
                            0.0)
                        .toString();
                final incomeId =
                    alloc['IncomeId'] ?? alloc['incomeId'] ?? 'N/A';
                final categoryId =
                    alloc['CategoryId'] ?? alloc['categoryId'] ?? 'N/A';

                return ListTile(
                  title: Text('ID: $id | Type: $type'),
                  subtitle: Text(
                    'Value: \$${value} | Income ID: $incomeId | Category ID: $categoryId',
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
