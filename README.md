# WCountAPI
WCountAPI is a set of word counting, character counting, byte counting, and line counting libraries and API.

## Projects within this repo

### WCount API
WCount API is MPL 2.0 licensed and uses WCountLib.

WCount API provides WCountLib word counting and character counting functionality over a web api.

### WCount Libraries
| Project Name | License | Description | 
|-|-|-|
| WCountLib.Abstractions | MPL 2.0 | A library to provide abstractions to enable other implementations of Word Counting etc. |
| WCountLib              | MPL 2.0 | A library to enable counting the number of lines, words, characters, and/or bytes in specified files, strings, or IEnumerables of strings. |
| WCountLib.Providers.wc | MPL 2.0 | Implements WCountLib Abstractions through programmatic use of the Unix ``wc`` command. Only supported on Unix based operating systems. |

