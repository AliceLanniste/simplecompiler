
### git  branch
新建一个分支，使用`git branch <name>`命令。
`git checkout <branchname>` 是切换到分支的命令，一旦切换，所有的提交记录，只会在这一分支上显示，
``` 
在B上新建一个分支，并切换到`topic`,E和F是后面提交的记录
    D-E-F (topic)
   /
A-B-C(master)
```

### git merge
以上面图为例子，要把两个分支合并到一起，`git merge topic`命令意思是，把`topic`分支合到`master`上，要合并分支，一定要先切换确定当前所在分支。

```  D-E-F (topic)
   /           \
A-B-C(master)-D(master*)

```
### git rebase
`git rebase <name>` 同样是合并分支和`git merge <name>` 不同在于，`git rebase`是基于线性，这样会使得整个提交记录更清晰。



```  D(topic)                    
   /               git rebase topic              
A-B-C(master)                      A-B-C-D(topic master*)
```


### git pull
### git push
