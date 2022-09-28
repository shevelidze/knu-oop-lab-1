# knu-oop-lab-1

## Technical task

Create an Excel-like application with a GUI, using C#.

### Data types

- Number
- String
- Boolean

### Formulas functionality

#### Links to other cells

```
A2
```

#### Mathematical operators

```
+, -, *, /, (), **
```

Some operators might be binary, and some might be unary.

#### Functions

```
function(arg1, arg2, arg3, ...);
```

#### Ability to combine all the previous expressions

```
(A1 + function1(100 * 200, "Hello", 5 / (2 + 3))) * function2(B5)
```

### Syntax highlighting

All the tokens in a formula are painted in the appropriate color. For example: booleans - red, function names - blue, numbers - yellow, etc.

## Parser organization

### Tokenizer
A part of the parser, that convers a random string to a list of definite tokens.

### Tree builder
The most massive part of the parser. Build an expressions tree from a tokens list.

### Executor
A part of the parser, that calculates the final value of the expression from an expressions tree.

Each part capable to throw errors.