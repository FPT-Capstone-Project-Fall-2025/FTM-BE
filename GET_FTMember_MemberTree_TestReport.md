# Test Report: GET /api/ftmember/member-tree

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetMembersTreeViewAsync |
| Function Name | Get Members Tree View |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 5 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 2 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 0 / 0 |
| **Total Test Cases** | **2** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| Access to system | O | O |
| Internet connection must be stable | O | O |
| Users have logged in | O | O |
| User has proper permission | O | O |

### Request

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| ftId is valid GUID | O | O |
| ftId exists in system | O | O |
| Family tree has members | O | - |
| Family tree has no members | - | O |
| Family tree has root member | O | - |
| Family tree has no root member | - | O |

### Service

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| Service.GetMembersTree() executes successfully | O | O |
| Service returns tree with root and data | O | - |
| Service returns empty tree (no root) | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| StatusCode: 200 | O | O |
| Returns ApiSuccess with message | O | O |
| Returns FTMemberTreeDto | O | O |
| Tree has Root | O | - |
| Tree has no Root | - | O |
| Tree has Datalist | O | - |
| Tree has empty Datalist | - | O |

### Exception

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| No exceptions | O | O |

### Log Message

| Condition | UTCID01 | UTCID02 |
|-----------|---------|---------|
| Lấy cây gia phả thành công | O | O |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | N | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed



