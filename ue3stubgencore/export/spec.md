# Introduction

In here we define our exportable content hierarchy. This covers the following:

- Classes
  - Inherited Classes (mono-inheritance)
  - Implemented Interfaces
  - can be abstract
  - Holds structure definitions
  - Holds enum definitions
  - Member variables
  - Holds member function definitions
  - Holds static function definitions
- Structs
  - can extend other structs
  - Holds member variables
- Enums
  - Constant values of an integral type identifiable by name
- Interfaces
  - always extend Interface
  - always abstract
  - Holds structure definitions
  - Holds enum definitions
  - Holds member function definitions
  - Holds static function definitions

This is described by the various classes in the `export` package.