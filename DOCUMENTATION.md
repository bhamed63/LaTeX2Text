# LaTeX Conversion Pipeline Documentation

This document provides an overview of the LaTeX parsing, extraction, and conversion pipeline.

## Part 1: The Parsing and Extraction Pipeline

The first part of the pipeline is responsible for converting a raw LaTeX string into a structured and analyzable format. This is accomplished by two key components: the `LatexParser` and the `LatexCommandExtractor`.

### `LatexParser`

The `LatexParser` is the core of the parsing engine. Its primary responsibility is to take a LaTeX string as input and produce an Abstract Syntax Tree (AST). The AST is a hierarchical tree structure that represents the grammatical and logical structure of the LaTeX input, making it easy to analyze and manipulate.

The parser is a recursive descent parser, which means it has a set of methods that correspond to the different grammatical structures of the LaTeX language. The main entry point is the `Parse` method, which iterates through the input string and delegates the parsing of specific structures to other methods, such as:

-   **`ParseCommand`**: This method is responsible for parsing LaTeX commands (e.g., `\frac`, `\sqrt`). It identifies the command's name and its arguments, and it has specialized logic to handle commands with unique structures, such as fractions, roots, and limit-style commands.
-   **`ParseText`**: This method handles plain text and is also responsible for parsing subscripts (`_`) and superscripts (`^`). It can correctly identify the base of a script and can handle chained scripts (e.g., `x_1^2`).
-   **`ParseGroup`**: This method is used to parse mathematical groups, such as those enclosed in `\[...\]`.

By the end of the parsing process, the `LatexParser` produces a list of `AstNode` objects, which together form the complete AST for the input string.

### `LatexCommandExtractor`

The `LatexCommandExtractor` is a utility class that works with the AST produced by the `LatexParser`. While the AST is a powerful and detailed representation of the LaTeX input, it can be complex to work with directly. The `LatexCommandExtractor` provides a way to simplify this structure by extracting a flat list of all the commands present in the AST.

The class has two main public methods:

-   **`ExtractCommands`**: This method traverses the AST and extracts a list of `CommandInfo` objects. Each `CommandInfo` object contains the name of a command and a list of its textual arguments. This is particularly useful for quickly identifying which commands are used in a given LaTeX string.
-   **`EnrichCommands`**: This method takes the list of `CommandInfo` objects and joins it with the `SymbolLibrary` (a dictionary of symbol definitions). The result is a list of `EnrichedCommandInfo` objects, where each object contains not only the command's name and arguments but also its full definition, including its plain text, screen reader, and human-friendly representations.

## Part 2: The Visitor Classes

Once the `LatexParser` has created an Abstract Syntax Tree (AST), the final step in the conversion pipeline is to traverse the AST and translate it into the desired output format. This is accomplished using the visitor design pattern, which allows for clean separation between the structure of the AST and the operations performed on it.

### Overview of the Visitor Pattern

The visitor pattern is implemented through a set of visitor classes that all inherit from the `BaseVisitor` abstract class. The `BaseVisitor` defines a `Visit` method for each type of node in the AST (e.g., `VisitText`, `VisitCommand`, `VisitScript`), which ensures that every visitor can handle every part of the AST.

When a visitor is passed an AST, it traverses the tree from the root to the leaves. At each node, it calls the appropriate `Visit` method, which is responsible for converting that specific node into a string. The final output is the concatenation of the strings produced by visiting each node in the tree.

### `HumanFriendlyVisitor`

The `HumanFriendlyVisitor` is designed to convert the AST into a format that is easy for humans to read. Its primary goal is to replace LaTeX commands with their corresponding Unicode symbols, which makes the output more visually appealing and intuitive.

For example, a LaTeX input like `\frac{1}{2} \alpha` would be converted to `1 / 2 α`.

### `ScreenReaderVisitor`

The `ScreenReaderVisitor` converts the AST into a format that is optimized for text-to-speech engines. The goal is to produce a string that, when read aloud, accurately describes the mathematical expression. This is achieved by replacing LaTeX commands with descriptive English words and phrases.

For example, a LaTeX input like `\frac{1}{2}` would be converted to `"fraction with numerator 1 and denominator 2"`, which is much clearer for a screen reader to interpret than the visual representation.

### `OpenAIVisitor`

The `OpenAIVisitor` is designed to convert the AST into a format that is easy for a large language model (LLM) to understand and process. This format is often a more explicit and less ambiguous representation of the LaTeX input, which helps the AI to correctly interpret the mathematical meaning.

For example, a LaTeX input like `\vec a` would be converted to `vec(a)`, which is a more programmatic and less ambiguous representation of a vector than the standard arrow notation.
