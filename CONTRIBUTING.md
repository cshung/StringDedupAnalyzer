## Contributing

First off, thank you for considering contributing to StringDedupAnalyzer. It's people like you that make StringDedupAnalyzer such a great tool.

### Where do I go from here?

If you've noticed a bug or have a feature request, [make one][new issue]! It's generally best if you get confirmation of your bug or approval for your feature request this way before starting to code.

### Fork & create a branch

If this is something you think you can fix, then [fork StringDedupAnalyzer] and create
a branch with a descriptive name.

A good branch name would be (where issue #325 is the ticket you're working on):

```sh
git checkout -b 325-count-long-strings
```

### Validating the fix

At this point, the tool does not come with any tests. Testing is done manually, and correctness is hard to tell. It would be really nice to have some tests.

### Make a Pull Request

At this point, you should switch back to your master branch and make sure it's
up to date with StringDedupAnalyzer's master branch:

```sh
git remote add upstream git@github.com:cshung/StringDedupAnalyzer.git
git checkout master
git pull upstream master
```

Then update your feature branch from your local copy of master, and push it!

```sh
git checkout 325-count-long-strings
git rebase master
git push --set-upstream origin 325-count-long-strings
```

Finally, go to GitHub and [make a Pull Request][] :D

### Keeping your Pull Request updated

If a maintainer asks you to "rebase" your PR, they're saying that a lot of code
has changed, and that you need to update your branch so it's easier to merge.

To learn more about rebasing in Git, there are a lot of [good][git rebasing]
[resources][interactive rebase] but here's the suggested workflow:

```sh
git checkout 325-count-long-strings
git pull --rebase upstream master
git push --force-with-lease 325-count-long-strings
```

[new issue]: https://github.com/cshung/StringDedupAnalyzer/issues/new
[fork StringDedupAnalyzer]: https://help.github.com/articles/fork-a-repo
[make a pull request]: https://help.github.com/articles/creating-a-pull-request
[git rebasing]: http://git-scm.com/book/en/Git-Branching-Rebasing
[interactive rebase]: https://help.github.com/en/github/using-git/about-git-rebase
