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
    // This function reloads data from the API
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
    // Controllers initialized with existing data
    final TextEditingController userIdController = TextEditingController(
      // Ensure UserId is correctly displayed for editing
      text: (category?['UserId'] ?? category?['userId'])?.toString() ?? '',
    );
    final TextEditingController nameController = TextEditingController(
      text: category?['CategoryName'] ?? category?['categoryName'] ?? '',
    );
    final TextEditingController descriptionController = TextEditingController(
      text: category?['Description'] ?? category?['description'] ?? '',
    );

    final int? categoryId = category?['CategoryId'] ?? category?['categoryId'];

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(category == null ? 'Add Category' : 'Edit Category'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // User ID Field
            TextField(
              controller: userIdController,
              decoration: const InputDecoration(labelText: 'User ID *'),
              keyboardType: TextInputType.number,
            ),
            // Name Field
            TextField(
              controller: nameController,
              decoration: const InputDecoration(labelText: 'Name *'),
            ),
            // Description Field
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
              final parsedUserId = int.tryParse(userIdController.text);

              // Input Validation
              if (nameController.text.isEmpty || parsedUserId == null) {
                _showSnackBar(
                  'Category Name and a valid User ID are required.',
                  isError: true,
                );
                return;
              }

              final data = {
                'UserId': parsedUserId, // Included for API request
                'CategoryName': nameController.text,
                'Description': descriptionController.text,
              };

              bool success = false;

              try {
                if (category == null) {
                  await ApiService.addCategory(data);
                  _showSnackBar('Category added successfully!');
                  success = true;
                } else {
                  if (categoryId != null) {
                    await ApiService.updateCategory(categoryId, data);
                    _showSnackBar('Category updated successfully!');
                    success = true;
                  }
                }
              } catch (e) {
                _showSnackBar(
                  'Failed to save category: ${e.toString()}',
                  isError: true,
                );
              }

              // ⭐ CRITICAL FIX: Only pop the dialog AND reload state IF successful.
              if (success) {
                if (mounted) Navigator.pop(context);
                setState(() => _loadCategories());
              }
              // If failed, the dialog remains open showing the error message.
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteCategory(int id) async {
    bool success = false;
    try {
      await ApiService.deleteCategory(id);
      _showSnackBar('Category deleted successfully!');
      success = true;
    } catch (e) {
      _showSnackBar(
        'Failed to delete category: ${e.toString()}',
        isError: true,
      );
    }

    if (success) {
      setState(() => _loadCategories());
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Categories')),
      body: FutureBuilder<List<dynamic>>(
        future: _categories,
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
              child: Text('No categories found. Tap + to add one.'),
            );
          } else {
            final categories = snapshot.data!;
            return ListView.builder(
              itemCount: categories.length,
              itemBuilder: (context, index) {
                final cat = categories[index];

                final name =
                    cat['CategoryName'] ??
                    cat['categoryName'] ??
                    'Unnamed Category';
                final description =
                    cat['Description'] ??
                    cat['description'] ??
                    'No description provided';
                final id = cat['CategoryId'] ?? cat['categoryId'];

                final userId = cat['UserId'] ?? cat['userId'] ?? 'N/A';

                return ListTile(
                  title: Text(name),
                  subtitle: Text('User ID: ${userId} | ${description}'),
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
