# Test Report: GET /api/ftmember/list

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetListOfMembers |
| Function Name | Get List of Family Tree Members |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 18 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 3 / 1 / 0 |
| **Total Test Cases** | **4** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
| UTCID03 | N (Normal) | P (Passed) | 06/02 |
| UTCID04 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Access to system | O | O | O | O |
| Internet connection must be stable | O | O | O | O |
| Users have logged in | O | O | O | O |
| User has proper permission | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| PageIndex is provided | O | O | O | O |
| PageSize is provided | O | O | O | O |
| Search parameter is provided | O | O | O | O |
| Search parameter finds results | O | - | O | - |
| Search parameter finds no results | - | O | - | - |
| PropertyFilters is provided | O | O | O | O |
| OrderBy is provided | O | O | O | O |
| Pagination parameters are valid | O | O | O | O |
| Multiple pages exist | - | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service.GetListOfMembers() executes successfully | O | O | O | - |
| Service.CountMembers() executes successfully | O | O | O | - |
| Service returns list with items | O | - | O | - |
| Service returns empty list | - | O | - | - |
| Service throws Exception | - | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | O | - |
| Returns ApiSuccess with message | O | O | O | - |
| Returns Pagination object | O | O | O | - |
| Pagination contains data | O | - | O | - |
| Pagination is empty | - | O | - | - |
| Pagination has correct TotalItems | O | O | O | - |
| Pagination has correct TotalPages | O | O | O | - |
| Pagination has correct PageIndex | O | O | O | - |
| Pagination has correct PageSize | O | O | O | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service Exception | - | - | - | O |
| Database connection error | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Lấy danh sách thành viên của gia phả thành công | O | O | O | - |
| Exception message | - | - | - | O |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | N | P | 06/02 | - |
| UTCID03 | N | P | 06/02 | - |
| UTCID04 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

