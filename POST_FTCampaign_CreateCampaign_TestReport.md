# Test Report: POST /api/ftcampaign

## Function Information

| Field | Value |
|-------|-------|
| Function Code | CreateCampaign |
| Function Name | Create Campaign |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 65 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 5 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 4 / 0 |
| **Total Test Cases** | **5** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID05 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Access to system | O | O | O | O | O |
| Internet connection must be stable | O | O | O | O | O |
| Users have logged in | O | O | O | O | O |
| User has proper permission | O | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Request body is provided | O | - | O | O | O |
| Request body is null | - | O | - | - | - |
| FamilyTreeId is valid GUID | O | - | O | - | O |
| FamilyTreeId is empty GUID | - | - | - | O | - |
| CampaignName is provided | O | - | O | O | O |
| CampaignManagerId is provided | O | - | - | O | O |
| CampaignManagerId is null | - | - | O | - | - |
| CampaignManagerId is empty GUID | - | - | O | - | - |
| StartDate is provided | O | - | - | - | O |
| EndDate is provided | O | - | - | - | O |
| FundGoal is provided | O | - | - | - | O |
| Bank information is provided | O | - | - | - | O |
| Request data is valid | O | - | - | - | - |
| Request data is invalid | - | O | O | O | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| CampaignService.AddAsync() executes successfully | O | - | - | - | - |
| CampaignService.AddAsync() throws Exception | - | - | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| StatusCode: 200 | O | - | - | - | - |
| StatusCode: 400 | - | O | O | O | O |
| Returns ApiSuccess with campaign info | O | - | - | - | - |
| Returns ApiError with message | - | O | O | O | O |
| Campaign status is Active | O | - | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| Request Body Is Null | - | O | - | - | - |
| Missing Campaign Manager ID | - | - | O | - | - |
| Missing Family Tree ID | - | - | - | O | - |
| Server Error | - | - | - | - | O |
| Exception thrown | - | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 |
|-----------|---------|---------|---------|---------|---------|
| CreateCampaign called | O | O | O | O | O |
| Request body is null | - | O | - | - | - |
| Campaign created successfully | O | - | - | - | - |
| Campaign manager ID is required | - | - | O | - | - |
| Family tree ID is required | - | - | - | O | - |
| Error creating campaign | - | - | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |
| UTCID04 | - |
| UTCID05 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | A | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |
| UTCID04 | A | P | 06/02 | - |
| UTCID05 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

