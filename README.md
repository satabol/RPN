# RPN

This is a simple library that can convert string in Func using RPN.

```csharp
   Func<double, double> func = Expression.ToDelegate("-log(x,2)*sin(x)");
```
and func - representation of the "-log(x,2)*sin(x)" as function of x.
