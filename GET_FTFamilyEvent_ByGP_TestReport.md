# Test Report: GET /api/ftfamilyevent/by-gp/{FTId}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetEventsByGP |
| Function Name | Get Events by Family Tree ID |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 17 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 1 / 1 |
| **Total Test Cases** | **4** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | B (Boundary) | P (Passed) | 06/02 |

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
| FTId is valid GUID | O | O | O | O |
| PageIndex is provided | O | O | O | O |
| PageSize is provided | O | O | O | O |
| PageIndex = 1 | O | O | O | - |
| PageIndex = 2 | - | - | - | O |
| PageSize = 10 | O | O | O | - |
| PageSize = 5 | - | - | - | O |
| Events exist for FTId | O | - | - | O |
| No events exist for FTId | - | O | - | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service.GetEventsByFTIdAsync() returns events | O | - | - | O |
| Service.GetEventsByFTIdAsync() returns empty list | - | O | - | - |
| Service.CountEventsByFTIdAsync() returns count | O | O | - | O |
| Service.GetEventsByFTIdAsync() throws Exception | - | - | O | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | O |
| StatusCode: 400 | - | - | O | - |
| Returns ApiSuccess with Pagination | O | O | - | O |
| Returns ApiError with message | - | - | O | - |
| Pagination contains data | O | - | - | O |
| Pagination is empty | - | O | - | - |
| Pagination has correct PageIndex | O | O | - | O |
| Pagination has correct PageSize | O | O | - | O |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Server Error | - | - | O | - |
| Exception thrown | - | - | O | - |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Events retrieved successfully | O | O | - | O |
| Server error message | - | - | O | - |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |
| UTCID04 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | N | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |
| UTCID04 | B | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

