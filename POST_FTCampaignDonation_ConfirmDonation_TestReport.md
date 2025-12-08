# Test Report: POST /api/ftcampaigndonation/{donationId}/confirm

## Function Information

| Field | Value |
|-------|-------|
| Function Code | ConfirmDonation |
| Function Name | Confirm Campaign Donation |
| Created By | BuuLD |
| Executed By | BuuLD |
| Lines of Code | 67 |
| Lack of Test Cases | - |

## Test Summary

| Status | Count |
|--------|-------|
| Passed | 6 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | 1 / 5 / 0 |
| **Total Test Cases** | **6** |

## Test Cases

| Test Case ID | Type | Result | Executed Date |
|--------------|------|--------|---------------|
| UTCID01 | N (Normal) | P (Passed) | 06/02 |
| UTCID02 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID03 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID04 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID05 | A (Abnormal) | P (Passed) | 06/02 |
| UTCID06 | A (Abnormal) | P (Passed) | 06/02 |

## Test Conditions and Coverage

### Precondition

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|---------|---------|---------|---------|---------|---------|
| Access to system | O | O | O | O | O | O |
| Internet connection must be stable | O | O | O | O | O | O |
| Users have logged in | O | O | O | O | O | O |
| User has proper permission | O | O | O | O | O | O |

### Request

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|---------|---------|---------|---------|---------|---------|
| Donation ID is provided in route | O | O | O | O | O | O |
| Donation ID is valid GUID | O | O | O | O | O | O |
| Request body is provided | O | O | O | O | O | O |
| DonationId in request matches route | O | - | O | O | O | O |
| DonationId in request does not match route | - | O | - | - | - | - |
| ConfirmedBy is provided | O | O | O | O | O | O |
| Notes is provided (optional) | O | O | O | O | O | O |
| Donation exists | O | - | - | O | O | O |
| Donation does not exist | - | - | O | - | - | - |
| Campaign exists | O | - | - | O | O | O |
| Campaign does not exist | - | - | - | - | - | - |
| ConfirmedBy matches Campaign Manager ID | O | - | - | - | O | O |
| ConfirmedBy does not match Campaign Manager ID | - | - | - | O | - | - |
| Donation status is Pending | O | - | - | - | - | O |
| Donation status is Completed | - | - | - | - | O | - |
| Donation has ProofImages | O | - | - | - | - | - |
| Donation does not have ProofImages | - | - | - | - | - | O |

### Service

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|---------|---------|---------|---------|---------|---------|
| DonationService.GetByIdAsync() returns donation | O | - | - | O | O | O |
| DonationService.GetByIdAsync() returns null | - | - | O | - | - | - |
| DonationService.GetCampaignForDonationAsync() returns campaign | O | - | - | O | O | O |
| DonationService.UpdateAsync() executes successfully | O | - | - | - | - | - |
| CampaignService.UpdateAsync() executes successfully | O | - | - | - | - | - |
| Donation status is updated to Completed | O | - | - | - | - | - |
| Campaign balance is updated | O | - | - | - | - | - |

### Confirm Return

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|---------|---------|---------|---------|---------|---------|
| StatusCode: 200 | O | - | - | - | - | - |
| StatusCode: 400 | - | O | - | - | O | O |
| StatusCode: 403 | - | - | - | O | - | - |
| StatusCode: 404 | - | - | O | - | - | - |
| Returns ApiSuccess with donation info | O | - | - | - | - | - |
| Returns ApiError with "Donation ID mismatch" | - | O | - | - | - | - |
| Returns ApiError with "Donation not found" | - | - | O | - | - | - |
| Returns ApiError with authorization message | - | - | - | O | - | - |
| Returns ApiError with "Donation already confirmed" | - | - | - | - | O | - |
| Returns ApiError with "Proof images are required" | - | - | - | - | - | O |
| Response includes DonationId | O | - | - | - | - | - |
| Response includes NewCampaignBalance | O | - | - | - | - | - |

### Exception

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|---------|---------|---------|---------|---------|---------|
| Donation ID Mismatch | - | O | - | - | - | - |
| Donation Not Found | - | - | O | - | - | - |
| Unauthorized (Not Campaign Manager) | - | - | - | O | - | - |
| Donation Already Confirmed | - | - | - | - | O | - |
| Proof Images Missing | - | - | - | - | - | O |

### Log Message

| Condition | UTCID01 | UTCID02 | UTCID03 | UTCID04 | UTCID05 | UTCID06 |
|-----------|---------|---------|---------|---------|---------|---------|
| Confirmed donation {DonationId} successfully | O | - | - | - | - | - |
| Donation ID mismatch | - | O | - | - | - | - |
| Donation not found | - | - | O | - | - | - |
| Only the Campaign Manager can confirm donations | - | - | - | O | - | - |
| Donation already confirmed | - | - | - | - | O | - |
| Proof images are required | - | - | - | - | - | O |

## Defect Information

| Test Case ID | Defect ID |
|--------------|-----------|
| UTCID01 | - |
| UTCID02 | - |
| UTCID03 | - |
| UTCID04 | - |
| UTCID05 | - |
| UTCID06 | - |

## Result

| Test Case ID | Type(N : Normal, A : Abnormal, B : Boundary) | Passed/Failed | Executed Date | Defect ID |
|--------------|-----------------------------------------------|---------------|---------------|-----------|
| UTCID01 | N | P | 06/02 | - |
| UTCID02 | A | P | 06/02 | - |
| UTCID03 | A | P | 06/02 | - |
| UTCID04 | A | P | 06/02 | - |
| UTCID05 | A | P | 06/02 | - |
| UTCID06 | A | P | 06/02 | - |

---

**Legend:**
- O = Covered by test case
- - = Not covered by test case
- N = Normal
- A = Abnormal
- B = Boundary
- P = Passed
- F = Failed

