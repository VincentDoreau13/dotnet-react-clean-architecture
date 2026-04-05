import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import { Auth0Provider } from "@auth0/auth0-react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { isApiError } from "@/api/catalog"
import "./index.css"
import App from "./App.tsx"

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      // Do not retry on 4xx responses — they are deterministic client errors.
      // Retry once on network failures (5xx / no response).
      retry: (failureCount, error) => {
        if (isApiError(error) && error.response !== undefined && error.response.status < 500) {
          return false
        }
        return failureCount < 1
      },
    },
  },
})

const rootElement = document.getElementById("root")
if (rootElement === null) {
  throw new Error("Root element #root not found in index.html")
}

createRoot(rootElement).render(
  <StrictMode>
    <Auth0Provider
      domain={import.meta.env.VITE_AUTH0_DOMAIN}
      clientId={import.meta.env.VITE_AUTH0_CLIENT_ID}
      authorizationParams={{
        redirect_uri: window.location.origin,
        audience: import.meta.env.VITE_AUTH0_AUDIENCE,
      }}
    >
      <QueryClientProvider client={queryClient}>
        <App />
      </QueryClientProvider>
    </Auth0Provider>
  </StrictMode>
)
