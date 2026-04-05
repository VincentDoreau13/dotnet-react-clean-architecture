import { useEffect, useState } from "react"
import { useAuth0 } from "@auth0/auth0-react"
import { apiClient } from "@/api/client"

export default function AxiosInterceptor({ children }: { children: React.ReactNode }) {
  const { getAccessTokenSilently } = useAuth0()
  const [ready, setReady] = useState(false)

  useEffect(() => {
    const interceptorId = apiClient.interceptors.request.use(async (config) => {
      const token = await getAccessTokenSilently()
      config.headers.Authorization = `Bearer ${token}`
      return config
    })

    setReady(true)

    return () => {
      apiClient.interceptors.request.eject(interceptorId)
      setReady(false)
    }
  }, [getAccessTokenSilently])

  if (!ready) return null

  return <>{children}</>
}
