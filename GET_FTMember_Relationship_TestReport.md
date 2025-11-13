# Test Report: GET /api/ftmember/{ftMemberId}/relationship

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetRelationship |
| Function Name | Get Member Relationship |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 4 |
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
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID03 | N (Normal) | P (Passed) | 06/02 |

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
| ftMemberId is valid GUID | O | O | O |
| ftMemberId exists in system | O | - | O |
| ftMemberId does not exist in system | - | O | - |
| Member has all relationships | O | - | - |
| Member has no parents | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Service.CheckRelationship() executes successfully | O | - | O |
| Service.CheckRelationship() throws ArgumentException (Member not found) | - | O | - |
| Service returns MemberRelationshipDto with all relationships | O | - | - |
| Service returns MemberRelationshipDto without parents | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | - | O |
| Returns ApiSuccess with message | O | - | O |
| Returns MemberRelationshipDto | O | - | O |
| HasFather is true | O | - | - |
| HasMother is true | O | - | - |
| HasPartner is true | O | - | - |
| HasChildren is true | O | - | - |
| HasFather is false | - | - | O |
| HasMother is false | - | - | O |
| HasSiblings is true | - | - | O |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Member Not Found | - | O | - |
| ArgumentException thrown | - | O | - |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| lấy mối quan hệ của thành viên trong gia phả thành công | O | - | O |
| Thành viên không tồn tại | - | O | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | A | P | 06/02 | - |
| UTCID03 | N | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed



