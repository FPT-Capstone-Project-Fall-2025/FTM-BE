# Test Report: DELETE /api/ftfamilyevent/{id}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | DeleteEvent |
| Function Name | Delete Family Tree Event |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 17 |
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
| id is valid GUID | O | O | O |
| Event exists in system | O | - | - |
| Event does not exist in system | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service.DeleteEventAsync() returns true | O | - | - |
| Service.DeleteEventAsync() returns false | - | O | - |
| Service.DeleteEventAsync() throws Exception | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | - | - |
| StatusCode: 404 | - | O | - |
| StatusCode: 400 | - | - | O |
| Returns ApiSuccess with message | O | - | - |
| Returns ApiError with message | - | O | O |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Event Not Found | - | O | - |
| Server Error | - | - | O |
| Exception thrown | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Event deleted successfully | O | - | - |
| Event not found | - | O | - |
| Server error message | - | - | O |

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



