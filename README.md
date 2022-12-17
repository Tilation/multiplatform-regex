# Regex on Repo
Or simply `ror` is a github action that does that.

This action only works on: `ubuntu` as it is a docker image.

### Example usage

```yaml
on:
  push:
    branches:
      - 'main'

jobs:
  regex:
    runs-on: ubuntu-latest
   
    steps:
    - name: Regex On Repo
      id: regex
      uses: tilation/regex-on-repo@ver      # replace ver with version number
      with:
        github: 'tilation/regex-on-repo'    # can use ${{ github.repository }}
        ref: 'main'                         # can use ${{ github.ref }}
        token: ${{ secrets.GITHUB_TOKEN }}
        regex: '`([^`\W]+)`'
        file: 'README.MD'
        debug: true
```


### Parameters:

| Parameter name | Parameter Type |Required | Parameter description | Examples |
|---            |---        |---    |---                                      |---|
| `github`      | string    | true  | The owner and repository that owns the file. | `"tilation/regex-on-repo"` |
| `ref`         | string    | true  | The reference to use for getting the file, can be a commit sha, branch name, or tag.| `"main"` or `"v1.5.3.2"` or a commit SHA |
| `token`       | string    | true  | A github token that has read access to the repository.| `${{ secrets.GITHUB_TOKEN }}` |
| `regex`       | string    | true  | The regex match string. | `"(\d+)\.(\d+)\.(\d+)\.(\d+)"` |
| `file`        | string    | true  | The directory to the file to read, relative to the root of the specified repository | `"README.MD"` |
| `debug`       | boolean   | false | Debug output toggle, will output to the console all the matches with every group. `false` by default. | `true` |


### Ouput Variables

The output is exported completely, matches and groups, and its like this:

I highly recommend playing arround with the `debug` flag set to true.

| Variable | Description | Example | Variable Type|
|---|---|---|---|
| matches | The amount of matches | | `int`
| match_X_groups | The amount of groups found in the Xth match, where X is a number starting from 0 (inclusive) | match_0_groups | `int`
| match_X_group_Y | The value for the Yth group of the Xth match, where X and Y are numbers starting from 0 (inclusive) | match_0_group_0 | `string`

