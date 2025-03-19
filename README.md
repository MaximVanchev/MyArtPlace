# MyArtPlace

## Overview

**MyArtPlace** is a creative platform designed for artists and art enthusiasts to showcase, discover, and share artwork. Whether you're a painter, photographer, digital artist, or simply an art lover, MyArtPlace provides a space to connect with a community that appreciates creativity. The platform allows users to upload their artwork, browse collections, and interact with other artists through comments and likes.

## Features

- **Artwork Upload**: Artists can upload their work with titles, descriptions, and tags.
- **Gallery View**: Browse artwork in a visually appealing gallery layout.
- **User Profiles**: Each user has a profile to display their uploaded artwork and bio.
- **Social Interaction**: Like and comment on artwork to engage with the community.
- **Search and Filter**: Easily search for artwork by tags, categories, or artist names.
- **Responsive Design**: The platform is fully responsive and works seamlessly on desktop and mobile devices.

## Technologies Used

- **React.js**: Frontend library for building the user interface.
- **Node.js**: Backend runtime environment.
- **Express.js**: Web framework for handling server-side logic.
- **MongoDB**: Database for storing user and artwork data.
- **Mongoose**: ODM (Object Data Modeling) library for MongoDB.
- **Cloudinary**: For image storage and management.
- **JWT (JSON Web Tokens)**: For user authentication and authorization.
- **Bootstrap**: For styling and responsive design.

## Getting Started

### Prerequisites

- Node.js (v14 or higher)
- MongoDB (v4.4 or higher)
- npm (Node Package Manager)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/MaximVanchev/MyArtPlace.git
   cd MyArtPlace
   ```

2. **Install dependencies**:
   ```bash
   npm install
   cd client
   npm install
   cd ..
   ```

3. **Set up environment variables**:
   Create a `.env` file in the root directory and add the following variables:
   ```env
   PORT=5000
   MONGODB_URI=mongodb://localhost:27017/myartplace
   JWT_SECRET=your_jwt_secret_key
   CLOUDINARY_CLOUD_NAME=your_cloudinary_cloud_name
   CLOUDINARY_API_KEY=your_cloudinary_api_key
   CLOUDINARY_API_SECRET=your_cloudinary_api_secret
   ```

4. **Run the server**:
   ```bash
   npm start
   ```

5. **Run the client**:
   Open a new terminal window, navigate to the `client` directory, and start the React app:
   ```bash
   cd client
   npm start
   ```

6. **Access the platform**:
   The platform will be running at `http://localhost:3000`.

## Usage

### For Artists

1. **Sign Up/Login**: Create an account or log in to your existing account.
2. **Upload Artwork**: Click on the "Upload" button to add your artwork. Provide a title, description, and tags.
3. **Manage Your Profile**: Update your profile information and view your uploaded artwork.

### For Art Enthusiasts

1. **Browse Artwork**: Explore the gallery to discover new artwork.
2. **Interact**: Like and comment on artwork to show your appreciation.
3. **Follow Artists**: Follow your favorite artists to stay updated with their latest work.

## Contributing

Contributions are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeatureName`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeatureName`).
5. Open a pull request.

## Acknowledgments

- Special thanks to all contributors who have helped improve this project.
- Inspired by the need for a platform where artists can share their work and connect with others.

## Contact

For any questions or suggestions, please feel free to reach out:

- **Maxim Vanchev**
- GitHub: [MaximVanchev](https://github.com/MaximVanchev)
- Email: maxim.van.mv@gmail.com

---

Thank you for using MyArtPlace! We hope it becomes your go-to platform for sharing and discovering amazing artwork. Happy creating! 🎨
