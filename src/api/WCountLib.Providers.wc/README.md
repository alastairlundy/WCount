## WCountLib.Providers.wc Library
This package provides implementations for WCountLib.Abstractions interfaces that use Posix's and Unix's ``wc`` program to perform the calculations.

The functionality in this package relies on the ``wc`` program which only supports Unix-based operating systems.

### Notes
Users of this library must configure CliInvoke with Dependency Injection when setting up the application.

For apps using ``Microsoft.Extensions.DependencyInjection`` or ``Microsoft.Extensions.Hosting``, install ``AlastairLundy.CliInvoke.Extensions``
and call the ``AddCliInvoke`` service collection extension method to set it up.

### Supported Platforms
The following table details which target platforms are supported for accessing WCountLib functionality via ``wc`.

| Operating System | Support Status                     | Notes                                                                        |
|------------------|------------------------------------|------------------------------------------------------------------------------|
| Windows          | Not supported :x:                  |                                                                              |
| macOS            | Fully Supported :white_check_mark: |                                                                              |
| Mac Catalyst     | Untested Platform :warning:        | Support for this platform has not been tested but should theoretically work. |
| Linux            | Fully Supported :white_check_mark: |                                                                              |
| FreeBSD          | Fully Supported :white_check_mark: |                                                                              |
| Android          | Untested Platform :warning:        | Support for this platform has not been tested but should theoretically work. |
| IOS              | Not Supported :x:                  |                                                                              | 
| tvOS             | Not Supported :x:                  |                                                                              |
| watchOS          | Not Supported :x:                  |                                                                              |


### Licensing
This library is licensed under the MPL 2.0 license.

If you'd like to contribute to the project, please visit the [GitHub Repo](https://github.com/alastairlundy/WCountAPI/).