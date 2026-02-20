# Higher-order function and closure examples

These examples demonstrate how to use:

- `ref(functionName)`
- `call(functionRef, ...args)`
- `closure(functionRef, ...capturedArgs)`

## Files

1. `01_apply_twice.txt`: Pass function as argument and apply it twice.
2. `02_make_adder_closure.txt`: Create closures that capture constants.
3. `03_affine_transform_factory.txt`: Build linear-transform functions with captured coefficients.
4. `04_newton_sqrt2.txt`: Approximate `sqrt(2)` using Newton's method with a closure-created step function.
5. `05_gradient_descent_quadratic.txt`: Optimize a quadratic using iterative gradient descent and a closure for learning rate.
6. `06_power_function_factory.txt`: Return function references to specialized recursive power functions.

You can run any file with the console app:

```bash
dotnet run --project AMLBS.ConsoleApp -- examples/04_newton_sqrt2.txt
```
