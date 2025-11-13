# Test Report: GET /api/ftfamilyevent/{eventId}/members

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetEventMembers |
| Function Name | Get Event Members |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 12 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 3 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 2 / 1 / 0 |
| **Total Test Cases** | **3** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | N (Normal) | P (Passed) | 06/02 |
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
| EventId is provided | O | O | O |
| EventId is valid GUID | O | O | O |
| Event exists | O | O | O |
| Event has members | O | - | - |
| Event has no members | - | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service.GetEventMembersAsync() executes successfully | O | O | - |
| Service returns list with items | O | - | - |
| Service returns empty list | - | O | - |
| Service throws Exception | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | O | - |
| StatusCode: 400 | - | - | O |
| Returns ApiSuccess with message | O | O | - |
| Returns ApiError with message | - | - | O |
| Returns list of members | O | - | - |
| Returns empty list | - | O | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service Exception | - | - | O |
| Server error | - | - | O |
| Event not found | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Event members retrieved successfully | O | O | - |
| Exception message | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed



