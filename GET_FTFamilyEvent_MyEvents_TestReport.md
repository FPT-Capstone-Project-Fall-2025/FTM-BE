# Test Report: GET /api/ftfamilyevent/my-events

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetMyEvents |
| Function Name | Get My Events |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 15 |
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
| User is authenticated | O | O | O | O |
| User has valid claims | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| FTId is provided | O | O | O | O |
| PageIndex is provided | O | O | O | O |
| PageSize is provided | O | O | O | O |
| PageIndex = 1 | O | O | O | - |
| PageIndex = 2 | - | - | - | O |
| PageSize = 10 | O | O | O | - |
| PageSize = 5 | - | - | - | O |
| UserId is extracted from claims | O | O | O | O |
| FTId is valid GUID | O | O | O | O |
| Pagination parameters are valid | O | O | O | O |
| Events exist for user | O | - | - | O |
| No events exist for user | - | O | - | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service.GetMyEventsAsync() executes successfully | O | O | - | O |
| Service returns list with items | O | - | - | O |
| Service returns empty list | - | O | - | - |
| Service throws Exception | - | - | O | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | O | - | O |
| StatusCode: 400 | - | - | O | - |
| Returns ApiSuccess with message | O | O | - | O |
| Returns ApiError with message | - | - | O | - |
| Returns list of events | O | - | - | O |
| Returns empty list | - | O | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Service Exception | - | - | O | - |
| Server error | - | - | O | - |
| Invalid UserId | - | - | O | - |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| My events retrieved successfully | O | O | - | O |
| Exception message | - | - | O | - |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |
| UTCID04 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

