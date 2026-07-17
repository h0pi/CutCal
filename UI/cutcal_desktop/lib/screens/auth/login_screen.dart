import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _usernameController = TextEditingController(text: 'admin');
  final _passwordController = TextEditingController(text: 'test');
  bool _isLoading = false;
  String? _usernameError;
  String? _passwordError;

  Future<void> _login() async {
    setState(() {
      _usernameError = _usernameController.text.isEmpty ? 'Username is required' : null;
      _passwordError = _passwordController.text.isEmpty ? 'Password is required' : null;
    });
    if (_usernameError != null || _passwordError != null) return;

    setState(() => _isLoading = true);
    try {
      await context.read<AuthProvider>().login(_usernameController.text, _passwordController.text);
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: SizedBox(
          width: 360,
          child: Card(
            child: Padding(
              padding: const EdgeInsets.all(32),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Icon(Icons.cut, size: 56, color: Colors.deepPurple),
                  const SizedBox(height: 8),
                  Text('CutCal Admin', style: Theme.of(context).textTheme.headlineSmall),
                  const SizedBox(height: 32),
                  TextField(
                    controller: _usernameController,
                    decoration: const InputDecoration(labelText: 'Username', border: OutlineInputBorder()),
                  ),
                  if (_usernameError != null)
                    Align(
                      alignment: Alignment.centerLeft,
                      child: Text(_usernameError!, style: const TextStyle(color: Colors.red, fontSize: 12)),
                    ),
                  const SizedBox(height: 16),
                  TextField(
                    controller: _passwordController,
                    obscureText: true,
                    decoration: const InputDecoration(labelText: 'Password', border: OutlineInputBorder()),
                    onSubmitted: (_) => _login(),
                  ),
                  if (_passwordError != null)
                    Align(
                      alignment: Alignment.centerLeft,
                      child: Text(_passwordError!, style: const TextStyle(color: Colors.red, fontSize: 12)),
                    ),
                  const SizedBox(height: 24),
                  SizedBox(
                    width: double.infinity,
                    child: FilledButton(
                      onPressed: _isLoading ? null : _login,
                      child: _isLoading
                          ? const SizedBox(height: 20, width: 20, child: CircularProgressIndicator(strokeWidth: 2))
                          : const Text('Log in'),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
