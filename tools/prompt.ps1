function prompt { "PS $($PWD.Path -replace '.+(?=\\)', '..')$('>' * ($nestedPromptLevel + 1)) " }
