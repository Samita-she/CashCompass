import 'dart:convert';
import 'package:http/http.dart' as http;

class ApiService {
  static const String baseUrl = 'http://127.0.0.1:5211';
  static const String apiPrefix = '/api';

  // -------------------- USERS --------------------
  static const String usersRoute = '$baseUrl$apiPrefix/Users';

  static Future<List<dynamic>> fetchUsers() async {
    final response = await http.get(Uri.parse(usersRoute));
    if (response.statusCode == 200) return jsonDecode(response.body);
    throw Exception('Failed to load users (Status ${response.statusCode})');
  }

  static Future<void> addUser(Map<String, dynamic> user) async {
    final response = await http.post(
      Uri.parse(usersRoute),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(user),
    );
    if (response.statusCode != 201)
      throw Exception('Failed to add user (Status ${response.statusCode})');
  }

  static Future<void> updateUser(int id, Map<String, dynamic> user) async {
    final response = await http.put(
      Uri.parse('$usersRoute/$id'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(user),
    );
    if (response.statusCode != 200)
      throw Exception('Failed to update user (Status ${response.statusCode})');
  }

  static Future<void> deleteUser(int id) async {
    final response = await http.delete(Uri.parse('$usersRoute/$id'));
    if (response.statusCode != 204)
      throw Exception('Failed to delete user (Status ${response.statusCode})');
  }

  // -------------------- CATEGORIES --------------------
  static const String categoriesRoute = '$baseUrl$apiPrefix/Categories';

  static Future<List<dynamic>> fetchCategories() async {
    final response = await http.get(Uri.parse(categoriesRoute));
    if (response.statusCode == 200) return jsonDecode(response.body);
    throw Exception(
      'Failed to load categories (Status ${response.statusCode})',
    );
  }

  static Future<void> addCategory(Map<String, dynamic> category) async {
    final response = await http.post(
      Uri.parse(categoriesRoute),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(category),
    );
    if (response.statusCode != 201)
      throw Exception('Failed to add category (Status ${response.statusCode})');
  }

  static Future<void> updateCategory(
    int id,
    Map<String, dynamic> category,
  ) async {
    final response = await http.put(
      Uri.parse('$categoriesRoute/$id'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(category),
    );
    if (response.statusCode != 200)
      throw Exception(
        'Failed to update category (Status ${response.statusCode})',
      );
  }

  static Future<void> deleteCategory(int id) async {
    final response = await http.delete(Uri.parse('$categoriesRoute/$id'));
    if (response.statusCode != 204)
      throw Exception(
        'Failed to delete category (Status ${response.statusCode})',
      );
  }

  // -------------------- ALLOCATIONS --------------------
  static const String allocationsRoute = '$baseUrl$apiPrefix/Allocations';

  static Future<List<dynamic>> fetchAllocations() async {
    final response = await http.get(Uri.parse(allocationsRoute));
    if (response.statusCode == 200) return jsonDecode(response.body);
    throw Exception(
      'Failed to load allocations (Status ${response.statusCode})',
    );
  }

  static Future<void> addAllocation(Map<String, dynamic> allocation) async {
    final response = await http.post(
      Uri.parse(allocationsRoute),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(allocation),
    );
    if (response.statusCode != 201)
      throw Exception(
        'Failed to add allocation (Status ${response.statusCode})',
      );
  }

  static Future<void> updateAllocation(
    int id,
    Map<String, dynamic> allocation,
  ) async {
    final response = await http.put(
      Uri.parse('$allocationsRoute/$id'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(allocation),
    );
    if (response.statusCode != 200)
      throw Exception(
        'Failed to update allocation (Status ${response.statusCode})',
      );
  }

  static Future<void> deleteAllocation(int id) async {
    final response = await http.delete(Uri.parse('$allocationsRoute/$id'));
    if (response.statusCode != 204)
      throw Exception(
        'Failed to delete allocation (Status ${response.statusCode})',
      );
  }

  // -------------------- INCOME SOURCES --------------------
  static const String incomeSourcesRoute = '$baseUrl$apiPrefix/IncomeSources';

  static Future<List<dynamic>> fetchIncomeSources() async {
    final response = await http.get(Uri.parse(incomeSourcesRoute));
    if (response.statusCode == 200) return jsonDecode(response.body);
    throw Exception(
      'Failed to load income sources (Status ${response.statusCode})',
    );
  }

  static Future<void> addIncomeSource(Map<String, dynamic> source) async {
    final response = await http.post(
      Uri.parse(incomeSourcesRoute),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(source),
    );
    if (response.statusCode != 201)
      throw Exception(
        'Failed to add income source (Status ${response.statusCode})',
      );
  }

  static Future<void> updateIncomeSource(
    int id,
    Map<String, dynamic> source,
  ) async {
    final response = await http.put(
      Uri.parse('$incomeSourcesRoute/$id'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(source),
    );
    if (response.statusCode != 200)
      throw Exception(
        'Failed to update income source (Status ${response.statusCode})',
      );
  }

  static Future<void> deleteIncomeSource(int id) async {
    final response = await http.delete(Uri.parse('$incomeSourcesRoute/$id'));
    if (response.statusCode != 204)
      throw Exception(
        'Failed to delete income source (Status ${response.statusCode})',
      );
  }

  // -------------------- EXPENSES --------------------
  static const String expensesRoute = '$baseUrl$apiPrefix/Expenses';

  static Future<List<dynamic>> fetchExpenses() async {
    final response = await http.get(Uri.parse(expensesRoute));
    if (response.statusCode == 200) return jsonDecode(response.body);
    throw Exception('Failed to load expenses (Status ${response.statusCode})');
  }

  static Future<void> addExpense(Map<String, dynamic> expense) async {
    final response = await http.post(
      Uri.parse(expensesRoute),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(expense),
    );
    if (response.statusCode != 201)
      throw Exception('Failed to add expense (Status ${response.statusCode})');
  }

  static Future<void> updateExpense(
    int id,
    Map<String, dynamic> expense,
  ) async {
    final response = await http.put(
      Uri.parse('$expensesRoute/$id'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(expense),
    );
    if (response.statusCode != 200)
      throw Exception(
        'Failed to update expense (Status ${response.statusCode})',
      );
  }

  static Future<void> deleteExpense(int id) async {
    final response = await http.delete(Uri.parse('$expensesRoute/$id'));
    if (response.statusCode != 204)
      throw Exception(
        'Failed to delete expense (Status ${response.statusCode})',
      );
  }
}
