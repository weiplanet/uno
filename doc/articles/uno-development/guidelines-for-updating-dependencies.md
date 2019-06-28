---
title: Guidelines for updating dependencies
---

We use Dependabot to notify the team of any updates to dependencies. Once a week the robot will scan our dependencies and raise a pull-request if a new version is found. If an existing open pull-request is found for a dependency it will be closed and replaced with a new pull-request. The behaviour of the robot is [controlled by this configuration file](https://github.com/nventive/Uno/blob/master/.dependabot/config.yml). 

## internal dependencies

These dependencies don't change the public API surface and are typically safe to merge and we could potentially [configure mergify to automatically merge them if CI passes](https://medium.com/mergify/merging-bots-pull-requests-automatically-548ed0b4a424):

- BenchmarkDotNet
- FluentAssertions 
- NUnit3TestAdapter 
- [NUnit.Runners](https://github.com/unoplatform/uno/pull/1122)
- [MSTest.TestAdapter](https://github.com/unoplatform/uno/pull/1126)
- [MSTest.TestFramework](https://github.com/unoplatform/uno/pull/1128)
- Moq
- [Xamarin.Build.Download](https://github.com/unoplatform/uno/pull/1123)

These dependencies require manual adjustments before merging:

- [docfx.console](https://github.com/nventive/Uno/pull/1082/commits/c222caf8c23b35e19f6b33cd624cbfa714250bfe)


## potentially incompatible with WASM

These dependancies require care and human testing to confirm compatibility with webassembly:

- [Microsoft.Extensions.Logging.Console](https://github.com/nventive/Uno/pull/894#issuecomment-495046929)

## chatops

You can trigger Dependabot actions by commenting on the pull-request:

```
@dependabot recreate will recreate this PR, overwriting any edits that have been made to it
@dependabot ignore this [patch|minor|major] version will close this PR and stop Dependabot creating any more for this minor/major version (unless you reopen the PR or upgrade to it yourself)
@dependabot ignore this dependency will close this PR and stop Dependabot creating any more for this dependency (unless you reopen the PR or upgrade to it yourself)
```

Please do not use any of the `rebase|merge|squash and merge` chatops commands as they bypass our merging pull-request guidelines and `ready-to-merge` workflow.
