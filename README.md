# AMLBS
Another Mathematical Language But Simpler. \
This is a minimal programming language that is probably useless.

## Language Resources:

* Expressions Evaluation: \
`1 + 1` \
`((5.5 - 3) * 5) / 7`

* Definition of Constants: \
`define const PI = 3.1415;` \
`define const TAU = 2 * PI;`

* Ternary Operator (or if function): \
`if(<expression>, <if true>, <if false>)` \
`if(1 == 1, 10, 7)`

* Definition of Functions: 

```
define function circleArea(r) = PI * r ^ 2;
> circleArea(5)
```

```
define function averageOf3(n1, n2, n3) = (n1 + n2 + n3) / 3;
> averageOf3(3, 6, 10)
```

* Recursive Functions: 
```
define function fat(x) = 
  if(x > 0, 
    x * fat(x-1)
  , 
    1
  );
```

```
define function fib(x) = 
  if (x == 0,
    0
  ,
    if (x == 1,
      1
    ,
      fib(x-1) + fib(x-2)
    )
  );
```

* Definition of Operators: \
`define operator(<precedence>) <name>(left, right) = <expression>;`
```
define operator(4) E(l, r) = l * (10 ^ r);
> 3 E 29
```
```
define operator(5) max(l, r) = if (l > r, l, r);
> 5 max 7
```
