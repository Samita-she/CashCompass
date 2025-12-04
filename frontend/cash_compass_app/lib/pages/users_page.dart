import 'dart:async';
import 'package:flutter/material.dart';
import '../api_service.dart';

class UsersPage extends StatefulWidget {
  const UsersPage({super.key});

  @override
  State<UsersPage> createState() => _UsersPageState();
}

class _UsersPageState extends State<UsersPage> {
  // Use a nullable Future to handle potential initial fetch errors more cleanly
  Future<List<dynamic>>? _users;

  @override
  void initState() {
    super.initState();
    _loadUsers();
  }

  void _loadUsers() {
    // Assigns the future to trigger the FutureBuilder
    _users = ApiService.fetchUsers();
  }

  // Helper to show SnackBar feedback
  void _showSnackBar(String message, {bool isError = false}) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: isError ? Colors.red.shade700 : Colors.green.shade700,
        duration: const Duration(seconds: 2),
      ),
    );
  }

  void _showUserDialog({Map<String, dynamic>? user}) {
    // Controllers initialized with existing data (PascalCase/camelCase safe) or blank
    final TextEditingController fullNameController = TextEditingController(
      text: user?['FullName'] ?? user?['fullName'] ?? '',
    );
    final TextEditingController emailController = TextEditingController(
      text: user?['Email'] ?? user?['email'] ?? '',
    );
    // Password controller is blank (NEVER read the hash back)
    final TextEditingController passwordController = TextEditingController();

    // Get ID safely for update
    final int? userId = user?['UserId'] ?? user?['userId'];

    showDialog(
      context: context,
      builder: (_) => AlertDialog(
        title: Text(user == null ? 'Add User' : 'Edit User'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: fullNameController,
                decoration: const InputDecoration(labelText: 'Full Name'),
              ),
              TextField(
                controller: emailController,
                decoration: const InputDecoration(labelText: 'Email'),
                keyboardType: TextInputType.emailAddress,
              ),
              // Security Fix: Password field for NEW password only
              TextField(
                controller: passwordController,
                decoration: InputDecoration(
                  labelText: user == null
                      ? 'Password (Required)'
                      : 'Password (Leave blank to keep existing)',
                ),
                obscureText: true,
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
              // Ensure we have basic data before proceeding
              if (fullNameController.text.isEmpty ||
                  emailController.text.isEmpty) {
                _showSnackBar(
                  'Full Name and Email are required.',
                  isError: true,
                );
                return;
              }

              if (user == null && passwordController.text.isEmpty) {
                _showSnackBar(
                  'Password is required for new users.',
                  isError: true,
                );
                return;
              }

              // Ensure keys sent to the C# API are PascalCase
              final data = {
                'FullName': fullNameController.text,
                'Email': emailController.text,
              };

              // Conditionally include PasswordHash ONLY if a value was entered
              if (passwordController.text.isNotEmpty) {
                data['PasswordHash'] = passwordController.text;
              }

              try {
                if (user == null) {
                  await ApiService.addUser(data);
                  _showSnackBar('User added successfully!');
                } else {
                  if (userId != null) {
                    await ApiService.updateUser(userId, data);
                    _showSnackBar('User updated successfully!');
                  }
                }
              } catch (e) {
                // Error Handling Fix: Show SnackBar on failure
                _showSnackBar(
                  'Failed to save user: ${e.toString()}',
                  isError: true,
                );
              }

              if (mounted) Navigator.pop(context);
              setState(() => _loadUsers());
            },
            child: const Text('Save'),
          ),
        ],
      ),
    );
  }

  void _deleteUser(int id) async {
    try {
      await ApiService.deleteUser(id);
      _showSnackBar('User deleted successfully!');
    } catch (e) {
      // Error Handling Fix: Show SnackBar on failure
      _showSnackBar('Failed to delete user: ${e.toString()}', isError: true);
    }
    setState(() => _loadUsers());
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Users (REST API)')),
      body: FutureBuilder<List<dynamic>>(
        // _users can be null if initState failed, but we assume it's set
        future: _users,
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
              child: Text('No users found. Tap + to add one.'),
            );
          } else {
            final users = snapshot.data!;
            return ListView.builder(
              itemCount: users.length,
              itemBuilder: (context, index) {
                final user = users[index];

                // Safely extract and display data
                final fullName =
                    user['FullName'] ?? user['fullName'] ?? 'Unnamed User';
                final email =
                    user['Email'] ?? user['email'] ?? 'No email provided';
                final id =
                    user['UserId'] ?? user['userId']; // Use the ID for actions

                return ListTile(
                  title: Text(fullName),
                  subtitle: Text(email),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      IconButton(
                        icon: const Icon(Icons.edit),
                        onPressed: () => _showUserDialog(user: user),
                      ),
                      IconButton(
                        icon: const Icon(Icons.delete),
                        onPressed: () {
                          if (id != null) {
                            _deleteUser(id);
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
        onPressed: () => _showUserDialog(),
        child: const Icon(Icons.add),
      ),
    );
  }
}
