// create a file named environment.ts and copy this object
// replace the values with your environment values

export const environment = {
    production: false,
    aadClientId: '87dd30bf-3ce2-4e43-8f6e-c327fb51517c', // The client app guid
    aadTenantId: 'df1a0638-43fa-47cd-b84d-da57d11717b9', // The azure tenant guid
    audience: '626a8756-7a42-46ee-b400-76de86669337',    // The registered unique app's URI part
    customApi: 'https://localhost:5001/api',
  };
  