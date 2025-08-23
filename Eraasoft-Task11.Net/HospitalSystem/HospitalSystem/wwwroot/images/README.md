# Doctor Images

This directory should contain doctor profile images referenced in the database.

## Image Naming Convention
- Images should be named exactly as stored in the Doctor.Img field in the database
- Supported formats: jpg, jpeg, png, gif
- Recommended size: 300x300 pixels or larger (square format preferred)

## Example
If a doctor record has `Img = "doctor1.jpg"`, place the image file as:
`wwwroot/images/doctor1.jpg`

## Fallback
If no image is found, the application will display a default medical icon placeholder.