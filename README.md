# VTEX Toolbelt

[![Build status](https://ci.appveyor.com/api/projects/status/yys7qdt4u3y3ybu1)](https://ci.appveyor.com/project/rdumont/toolbelt-dotnet)

## Requisitos

* Windows 7 ou superior
* .NET Framework 4.5

## Instalação

1. Baixe a [versão mais atual do código fonte](https://github.com/vtex/toolbelt-dotnet/archive/master.zip), ou clone o repositório.
2. Na pasta do projeto, usando o Bash, execute `make`.
3. Se quiser, crie um _alias_ chamado `vtex` que aponte para `<path_to_root>/src/Cli/bin/Release/vtex.exe`.

## Uso

```sh
$ vtex sync -a <accountName> -s <session>
```

O comando `sync` por padrão usará a pasta `/<accountName>` relativo ao diretório corrente. Ele começará ressincronizando o diretório remoto com os dados locais, para que estejam iguais.