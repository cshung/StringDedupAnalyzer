# StringDedupAnalyzer

A data collection tool to support the [StringDedup feature](https://github.com/dotnet/runtime/blob/master/docs/design/features/StringDeduplication.md).

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. 

### Prerequisites

.NET Core 3.1 is required, it can be installed [here](https://dotnet.microsoft.com/download). 

### Compiling

Set your working directory to `<repo_root>/src`

```
cd src
```

And then build using the `dotnet` command

```
dotnet build
```

## Running the code

Set your working directory to `<repo_root>/src/StringDedupAnalyzer`

```
cd src/StringDedupAnalyzer
```

and then execute the tool as follow

```
dotnet run path/to/dmp
```

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our [code of conduct](CODE-OF-CONDUCT.md), and the process for submitting pull requests to us.

In addition to code fixes, we are actively looking for data. Please take a look at the [data collection page](DataCollection.md) for more information.

## Authors

* **Andrew Au** - *Initial work* - [cshung](https://github.com/cshung)

See also the list of [contributors](https://github.com/cshung/StringDedupAnalyzer/graphs/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

* The project depends on [Microsoft.Diagnostics.Runtime](https://github.com/Microsoft/clrmd) for managed heap analysis.
