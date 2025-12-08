# Test Report: POST /api/funds

## Function Information

| Field | Value |
|-------|-------|
| Function Code | CreateFund |
| Function Name | Create Family Fund |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 26 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 3 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 2 / 0 |
| **Total Test Cases** | **3** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Access to system | O | O | O |
| Internet connection must be stable | O | O | O |
| Users have logged in | O | O | O |
| User has proper permission | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Request body is provided | O | O | O |
| FamilyTreeId is valid GUID | O | O | O |
| FundName is provided | O | O | - |
| FundName is empty | - | - | O |
| Description is provided | O | O | O |
| Bank information is provided | O | O | - |
| Request data is valid | O | - | - |
| Request data is invalid | - | O | O |
| Family tree exists | O | - | - |
| Family tree does not exist | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service.CreateFundAsync() executes successfully | O | - | - |
| Service.CreateFundAsync() throws Exception (Family tree not found) | - | O | - |
| Service.CreateFundAsync() throws Exception (Invalid data) | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | - | - |
| StatusCode: 500 | - | O | O |
| Returns ApiSuccess with FundId | O | - | - |
| Returns ApiError with message | - | O | O |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Family Tree Not Found | - | O | - |
| Invalid Data | - | - | O |
| Exception thrown | - | O | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Created new fund {FundId} for tree {TreeId} | O | - | - |
| Error creating fund for tree | - | O | O |
| Family tree not found | - | O | - |
| Fund name is required | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | A | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

