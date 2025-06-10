export const environment = {
  production: false,
  authApiUrl: 'https://localhost:5050',
  foodCatalogApiUrl: 'https://localhost:5051',
  foodRecordsApiUrl: 'https://localhost:5052',
  recipeApiUrl: 'https://localhost:5053',
  // recommendationApiUrl: 'http://localhost:5003',
  // Qdrant Vector DB (optional if UI/backend interacts directly)
  // qdrantUrl: 'http://localhost:6333'
  firebase: {
    apiKey: "AIzaSyCsAv2yyGdak1LTojK39FNsyYbnjfAGwxA",
    authDomain: "nutrimatrix-auth.firebaseapp.com",
    projectId: "nutrimatrix-auth",
    storageBucket: "nutrimatrix-auth.firebasestorage.app",
    messagingSenderId: "278182321457",
    appId: "1:278182321457:web:4e7693dd6a01ba0a6b2410"
  }
};
