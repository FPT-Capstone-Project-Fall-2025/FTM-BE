# EventType Enum

## Các loại sự kiện hỗ trợ:

| Giá trị | Mô tả | Gợi ý màu sắc | Gợi ý icon |
|---------|-------|---------------|------------|
| `Memorial` (1) | Ma chay, giỗ | Tím/Purple | 🎓 |
| `Wedding` (2) | Cưới hỏi | Xanh lá/Green | 💚 |
| `Birthday` (3) | Sinh nhật | Xanh dương/Blue | 🎂 |
| `Other` (4) | Khác | Đỏ/Red | 🎉 |

## Cách sử dụng trong API:

### Request (tạo sự kiện):
```json
POST /api/FTFamilyEvent
{
  "name": "Đám cưới Anh Văn",
  "eventType": "Wedding",  // Hoặc số: 2
  "startTime": "2024-12-25T10:00:00Z",
  "endTime": "2024-12-25T15:00:00Z",
  ...
}
```

### Response:
```json
{
  "id": "...",
  "name": "Đám cưới Anh Văn",
  "eventType": "Wedding",  // Trả về dạng string
  ...
}
```

## Notes:
- API chấp nhận cả string name (`"Wedding"`) hoặc số (`2`)
- Response luôn trả về string name
- Case-insensitive khi gửi request
