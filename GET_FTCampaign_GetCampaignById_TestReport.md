# Test Report: GET /api/ftcampaign/{id}

## Function Information

| Field | Value |
|-------|-------|
| Function Code | GetCampaignById |
| Function Name | Get Campaign By ID |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 83 |
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
| Campaign ID is provided in route | O | O | O |
| Campaign ID is valid GUID | O | O | O |
| Campaign exists | O | - | - |
| Campaign does not exist | - | O | - |
| Campaign ID is invalid | - | - | - |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| CampaignService.GetByIdAsync() returns campaign | O | - | - |
| CampaignService.GetByIdAsync() returns null | - | O | - |
| CampaignService.GetByIdAsync() throws Exception | - | - | O |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| StatusCode: 200 | O | - | - |
| StatusCode: 404 | - | O | - |
| StatusCode: 400 | - | - | O |
| Returns ApiSuccess with campaign data | O | - | - |
| Returns ApiError with "Campaign not found" | - | O | - |
| Returns ApiError with exception message | - | - | O |
| Campaign data includes Id, FTId, CampaignName | O | - | - |
| Campaign data includes Donations list | O | - | - |
| Campaign data includes Expenses list | O | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Campaign Not Found | - | O | - |
| Server Error | - | - | O |
| Exception thrown | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 |
|-----------|---------|---------|---------|
| Retrieved campaign {CampaignId} successfully | O | - | - |
| Campaign {CampaignId} not found | - | O | - |
| Error retrieving campaign {CampaignId} | - | - | O |
| Server error | - | - | O |

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

