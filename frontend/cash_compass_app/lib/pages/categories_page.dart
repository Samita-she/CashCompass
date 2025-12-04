import 'dart:async';
import 'package:flutter/material.dart';
import '../api_service.dart';

class CategoriesPage extends StatefulWidget {
  const CategoriesPage({super.key});

  @override
  State<CategoriesPage> createState() => _CategoriesPageState();
}

class _CategoriesPageState extends State<CategoriesPage> {
  Future<List<dynamic>>? _categories;

  @override
  void initState() {
    super.initState();
    _loadCategories();
  }

  void _loadCategories() {
    _categories = ApiService.fetchCategories();
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

  void _showCategoryDialog({Map<String, dynamic>? category}) {
    // Controllers initialized with existing data (PascalCase/camelCase safe)
    final TextEditingController nameController = TextEditingController(
      text: category?['CategoryName'] ?? category?['categoryName'] ?? '',
    );
    final TextEditingController descriptionController = TextEditingController(
      text: category?['Description'] ?? category?['description'] ?? '',
    );

    // Ensure ID is read safely for updates
    final int? categoryId = category?['CategoryId'] ?? category?['categoryId'];

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(category == null ? 'Add Category' : 'Edit Category'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: nameController,
              decoration: const InputDecoration(labelText: 'Name *'),
            ),
            TextField(
              controller: descriptionController,
              decoration: const InputDecoration(labelText: 'Description'),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () async {
              // Validation Fix: Check required fields
              if (nameController.text.isEmpty) {
                _showSnackBar('Category Name is required.', isError: true);
                return;
              }

              final data = {
                'CategoryName': nameController.text,
                'Description': descriptionController.text,
              };

              try {
                if (category == null) {
                  await ApiService.addCategory(data);
                  _showSnackBar('Category added successfully!');
                } else {
                  if (categoryId != null) {
                    await ApiService.updateCategory(categoryId, data);
                    _showSnackBar('Category updated successfully!');
                  }
                }
              } catch (e) {
                // Error Handling Fix: Show SnackBar on failure
                _showSnackBar(
                  'Failed to save category: ${e.toString()}',
                  isError: true,
                );
              }

              if (mounted) Navigator.pop(context);
              setState(() => _loadCategories());
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteCategory(int id) async {
    try {
      await ApiService.deleteCategory(id);
      _showSnackBar('Category deleted successfully!');
    } catch (e) {
      // Error Handling Fix: Show SnackBar on failure
      _showSnackBar(
        'Failed to delete category: ${e.toString()}',
        isError: true,
      );
    }
    setState(() => _loadCategories());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Categories (REST API)')),
      body: FutureBuilder<List<dynamic>>(
        future: _categories,
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
              child: Text('No categories found. Tap + to add one.'),
            );
          } else {
            final categories = snapshot.data!;
            return ListView.builder(
              itemCount: categories.length,
              itemBuilder: (context, index) {
                final cat = categories[index];

                // Safely extract and display data
                final name =
                    cat['CategoryName'] ??
                    cat['categoryName'] ??
                    'Unnamed Category';
                final description =
                    cat['Description'] ??
                    cat['description'] ??
                    'No description provided';
                final id = cat['CategoryId'] ?? cat['categoryId'];

                return ListTile(
                  title: Text(name),
                  subtitle: Text(description),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: const Icon(Icons.edit),
                        onPressed: () => _showCategoryDialog(category: cat),
                      ),
                      IconButton(
                        icon: const Icon(Icons.delete),
                        onPressed: () {
                          if (id != null) {
                            _deleteCategory(id);
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
        onPressed: () => _showCategoryDialog(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
