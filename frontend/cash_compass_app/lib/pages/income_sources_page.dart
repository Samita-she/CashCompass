import 'dart:async';
import 'package:flutter/material.dart';
import '../api_service.dart';

class IncomeSourcesPage extends StatefulWidget {
  const IncomeSourcesPage({super.key});

  @override
  State<IncomeSourcesPage> createState() => _IncomeSourcesPageState();
}

class _IncomeSourcesPageState extends State<IncomeSourcesPage> {
  Future<List<dynamic>>? _incomeSources;

  @override
  void initState() {
    super.initState();
    _loadIncomeSources();
  }

  void _loadIncomeSources() {
    _incomeSources = ApiService.fetchIncomeSources();
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

  void _showIncomeSourceDialog({Map<String, dynamic>? source}) {
    final TextEditingController userIdController = TextEditingController(
      text: (source?['UserId'] ?? source?['userId'])?.toString() ?? '',
    );
    final TextEditingController nameController = TextEditingController(
      text: source?['SourceName'] ?? source?['sourceName'] ?? '',
    );
    final TextEditingController amountController = TextEditingController(
      text: (source?['Amount'] ?? source?['amount'])?.toString() ?? '',
    );
    final TextEditingController frequencyController = TextEditingController(
      text: source?['PayFrequency'] ?? source?['payFrequency'] ?? '',
    );

    final TextEditingController nextPayDateController = TextEditingController(
      text: source?['NextPayDate'] ?? source?['nextPayDate'] ?? '',
    );

    // Get ID safely for update
    final int? incomeId = source?['IncomeId'] ?? source?['incomeId'];

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(
          source == null ? 'Add Income Source' : 'Edit Income Source',
        ),
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
                controller: nameController,
                decoration: const InputDecoration(labelText: 'Source Name *'),
              ),
              TextField(
                controller: amountController,
                decoration: const InputDecoration(labelText: 'Amount *'),
                keyboardType: TextInputType.number,
              ),
              TextField(
                controller: frequencyController,
                decoration: const InputDecoration(labelText: 'Pay Frequency'),
              ),
              TextField(
                controller: nextPayDateController,
                decoration: InputDecoration(
                  labelText: 'Next Pay Date (YYYY-MM-DD)',
                  suffixIcon: IconButton(
                    icon: const Icon(Icons.calendar_today),
                    onPressed: () => _selectDate(nextPayDateController),
                  ),
                ),
                readOnly: true,
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
                  nameController.text.isEmpty ||
                  parsedAmount == null ||
                  parsedAmount <= 0) {
                _showSnackBar(
                  'Please fill in User ID, Source Name, and a valid Amount.',
                  isError: true,
                );
                return;
              }

              // FIX 1: Ensure date is ALWAYS YYYY-MM-DD for C# backend
              // If empty, default to today's date, formatted cleanly.
              final String nextPayDate = nextPayDateController.text.isNotEmpty
                  ? nextPayDateController.text
                  : DateTime.now().toIso8601String().substring(0, 10);

              final data = {
                'UserId': parsedUserId,
                'SourceName': nameController.text,
                'Amount': parsedAmount,
                'PayFrequency': frequencyController.text,
                'NextPayDate': nextPayDate, // Use the fixed date string
              };

              // FIX 2: Implement success flag logic to prevent crash
              bool success = false;

              try {
                if (source == null) {
                  await ApiService.addIncomeSource(data);
                  _showSnackBar('Income Source added successfully!');
                  success = true;
                } else {
                  if (incomeId != null) {
                    await ApiService.updateIncomeSource(incomeId, data);
                    _showSnackBar('Income Source updated successfully!');
                    success = true;
                  }
                }
              } catch (e) {
                _showSnackBar(
                  'Failed to save Income Source: ${e.toString()}',
                  isError: true,
                );
              }

              // Only pop and reload if the operation succeeded
              if (mounted) Navigator.pop(context);
              if (success) {
                setState(() => _loadIncomeSources());
              }
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteIncomeSource(int id) async {
    bool success = false;
    try {
      await ApiService.deleteIncomeSource(id);
      _showSnackBar('Income Source deleted successfully!');
      success = true;
    } catch (e) {
      _showSnackBar(
        'Failed to delete Income Source: ${e.toString()}',
        isError: true,
      );
    }
    if (success) {
      setState(() => _loadIncomeSources());
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Income Sources')),
      body: FutureBuilder<List<dynamic>>(
        future: _incomeSources,
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
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(
              child: Text('No income sources found. Tap + to add one.'),
            );
          } else {
            final sources = snapshot.data!;
            return ListView.builder(
              itemCount: sources.length,
              itemBuilder: (context, index) {
                final src = sources[index];

                // Safely extract and display data
                final name =
                    src['SourceName'] ?? src['sourceName'] ?? 'Unnamed Source';
                final amount = (src['Amount'] ?? src['amount'] ?? 0.0)
                    .toString();
                final id = src['IncomeId'] ?? src['incomeId'];

                return ListTile(
                  title: Text(name),
                  subtitle: Text(
                    'Amount: \$${amount} | Freq: ${src['PayFrequency'] ?? src['payFrequency']} | Next: ${src['NextPayDate'] ?? src['nextPayDate'] ?? 'N/A'}',
                  ),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: const Icon(Icons.edit),
                        onPressed: () => _showIncomeSourceDialog(source: src),
                      ),
                      IconButton(
                        icon: const Icon(Icons.delete),
                        onPressed: () {
                          if (id != null) {
                            _deleteIncomeSource(id);
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
        onPressed: () => _showIncomeSourceDialog(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
