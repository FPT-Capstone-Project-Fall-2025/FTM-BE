# Test Report: Update Screening Evaluation Report

## Function Information

| Field | Value |
|-------|-------|
| Function Code | UpdateScreeningEvaluationReport |
| Function Name | Update Screening Evaluation Report |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 90 |
| Lack of Test Cases | 23, 72 |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 3 / 0 |
| **Total Test Cases** | **4** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Access to system | O | O | O | - |
| Internet connection must be stable | O | O | O | - |
| Users have logged in | O | O | O | - |
| User has proper permission | O | O | O | - |

### Reception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Reception ID greater than 0 | O | - | O | O |
| Reception ID is less than or equal to 0 | - | O | - | - |

### ScreeningEvaluation

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| command.Id == {id} | O | - | O | O |
| command.Id != {id} | - | O | - | - |
| Valid ScreeningEvaluation Id | O | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| StatusCode: 200 | O | - | - | - |
| StatusCode: 400 | - | O | - | O |
| StatusCode: 404 | - | - | O | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Invalid ReceptionId | - | O | - | - |
| Not Found | - | - | O | - |
| Unauthorize | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 |
|-----------|---------|---------|---------|---------|
| Updated successfully | O | - | - | - |
| Not found | - | - | O | O |
| Unauthorize | - | - | - | O |

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

