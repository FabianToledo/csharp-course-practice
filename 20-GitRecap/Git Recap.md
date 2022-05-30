How do we create an empty git rempository:

`git init`

Overview about what is going on in the git folder

`git status`

Add file contents to the index.

`git add <pathspec>`  
`git add .` This directory

NOTE: `git rm --cached <file>` to unstage new files

Commit the changes:

`git commit -m "This commit message"`

For the message use:
" on windows
' on linux

--------------------------------------
Now we can make a change in our files and see what happens:  

`git status`
will show us the modified files

now, we have 2 posibilities:  
`git add <file>`  
`git restore <file>`  

What git add does is: adds a creation, modification or deletion of files to the Index!

Note: `git restore --staged <file>` to unstage

`git commit -m "Modified some files"`

------------------------------------------------

Show the commit logs (history of commits)  
`git log`

Show the details of a commit:  
`git show <partial_commit_hash>` this will show the commit that matches the hash  
`git show HEAD` this will show the HEAD commit  
`git show HEAD~1` this will show one commit before the HEAD commit  
`git show <branch_name>` this will show the tip of a branch  

-----------------------------------------------
Show changes between commits, commit and working tree, etc

* Show the differences between the working tree and the index.  
`git diff`  

* Show  the differences between the working tree and a named <commit>
git diff <commit>  
e.g.  
`git diff <partial_commit_hash>`  
`git diff HEAD~1`  
`git diff HEAD`  
`git diff <branch_name>`  

* Show the changes between two arbitrary commit.  
`git diff <commit_from> <commit_to>`  
if we are standing on the HEAD commit:  
`git diff HEAD~1`  
is equivalent to  
`git diff HEAD~1 HEAD`  

-----------------------------------------------

Adding a tag to the current commit:  
`git tag <tag_name>`

Show all the tags:  
`git tag`

-----------------------------------------------

Stash:
-----
When we are in the middle of a feature and we have to change to another branch, for instance because there is a bug and we have to fix it. We can use stash to save the current work to retrieve it later.
The stash is a stack.

`git stash push -u -m "Message to identify the stash"`  
-u include untracked files

To list all the stashes:  
`git stash list`

To inspect a stash:  
`git stash show`  
`git stash show [<stash>]`  

To apply the last stash (this will delete the stash if there are no conflicts)
`git stash pop`  

Apply the last stash but do not delete it  
`git stash apply`  
Apply the a stash but do not delete it  
`git stash apply <stash>`  

To delete the last stash:  
`git stash drop`  
To delete a last stash:  
`git stash drop <stash>`  

-------------------------------------------------

Colaboration:
------------

Remotes:
e.g. (esta dir usa ssh que ya estaba configurado)

`git clone git@github.com:FabianToledo/minutas_titlin.git`  

También se puede utilizar la autentificación https:

`git clone https://github.com/FabianToledo/minutas_titlin.git`  

The command git remote will list all the remotes:

`git remote`

> origin

`git remote get-url <name-of-remote>`

`git remote get-url origin`

> git@github.com:FabianToledo/minutas_titlin.git

We could have multiple remotes.
And use this to synchronize different remotes.

We can add a remote using:
`git remote add <name> <url>`

Test creating a new repo in github and copy the url:

`git remote add origin git@github.com:FabianToledo/git-recap.git`

--------------------------------------------------
To sincronize the working tree with the contents of the remote (on the current branch)

`git pull`

this command is a 2 step process, it does

`git fetch` (this command checks what is new but does not get anything)

`git pull`  (gets the changes on the current branch)

--------------------------------------------------

`git push` (take the local contents and send them to the remote)

for the push command to work, we need that the branch exists (upstream branch) in the remote and to be tracked.

When we clone, this info is already in the remote, but when we have a new branch and we want to send it to the remote, we will have to set the upstream branch.

`git branch --set-upstream-to=<remote>/<branch> <local_branch>`

`git branch --set-upstream-to=origin/main main`

then we can execute git push or git pull

if we are pushing we can also do it in one step:

`git push --set-upstream origin main`  
or  
`git push -u origin main`  

--------------------------

Features (branching):
---------------------

`git branch feature1`  will create the branch called feature1

`git branch`  lists the all the branches

`  feature1`  
`* main`  

the asterix marks which branch is checked-out

`git switch feature1`  to checkout the branch named feature1

We can create and switch in one step, using -c (create)

`git switch -c feature2`

Merge:
-----

Switch to the __target__ branch

`git switch main`  

merge changes from the branche feature1:

`git merge feature1`

Conflicts:
---------

If 2 branches changes the same line or lines in a file, when these 
branches are merged into the main branch there will be some conflicts.

The conflicts are marked with the headers <<<<<<< HEAD, =======, >>>>>>> <commit>
e.g.
```
<<<<<<< HEAD  (Current change)
console.log('Line changed by the 1st commit');
=======
console.log('Line changed by the 2nd commit');
>>>>>>> <commit>  (Incoming change)
```

The default format does not show what was the line in the original commit
An alternative style can be used by setting the `merge.conflictStyle`
configuration variable to either "diff3" or "zdiff3"

**zdiff3 style:**

```
<<<<<<< HEAD  (Current change)
console.log('Line changed by the 1st commit');
||||||| <base>
console.log('Original line');
=======
console.log('Line changed by the 2nd commit');
>>>>>>> <commit>  (Incoming change)
```

If we decide that the conflicts are not resolvable, we can execute:

`git merge --abort´

But if we can resolve the conflict, we have to edit the file ande keep the desired change, both, none or a different solution.
Save the file and commit with `git add .` and `git commit -m "Commit message"`

-----------------------------------------------

Recommendation:
--------------

* Branch early  
* Commit often  
* Merge or Rebase often  

One typical scenario of daily work:

1- Branch to start working. `git switch -c new_feature`  
2- During the course of the day, make Commits `git commit -m "message"`  
3- The next day, the first thig you should do is a fetch of the main (or develop) branch and if there are new commits in that branch, merge those changes into your branch or rebase your branch into the head of the main (or develop).  
`git merge main` gets the last changes into your branch.  
OR  
`git rebase main` this will rebase your branch on top of the main branch.  

-------------------------------------------------

Github flow:
-----------

[Github flow](https://docs.github.com/es/get-started/quickstart/github-flow)

Github flow is only a workflow. It is used to collaborate between collaborators of a repo.

1- Create a branch

By creating a branch, we create a space to work and give collaborators a chance to review the code.

2- Make changes

Commit and push the changes to the branch. Give a descriptive message. Ideally, each commit contains an isolated complete change. Continue to make, commit, and push changes to your branch until you are ready to ask for feedback.

3- Create a pull request

Why is called a pull request? Because we are __not__ pushing our changes to the main or production branch, we are asking to the owner or the team to __pull__ our changes to the main branch.
Crate a pull request to ask collaborators for feedback on your changes and what problem they solve.  
Include images, links and anything to help others to understand.

4- Address review comments

Reviewers should leave questions, comments and suggestions.
You can continue to commit and push changes in response to the reviews. The pull request will update automatically.

5- Deploy for testing.

6- Merge your pull request

Once your pull request is approved, merge your pull request. 
The collaborators who has access can pull and merge the changes.

7- Delete your branch

After you merge your pull request, delete your branch.

----------------------------------------------------

Forking:
-------

If you want to contribute to a project that you are not part of, i.e. you do not have write permission.
How to propose a code change?

[Fork a Repo](https://docs.github.com/es/get-started/quickstart/fork-a-repo)

[Sync a Fork](https://docs.github.com/es/pull-requests/collaborating-with-pull-requests/working-with-forks/syncing-a-fork)

