import type { ReactNode } from "react"
import { Link, useLocation } from "react-router-dom"
import { ShoppingBag, LogOut } from "lucide-react"
import { useAuth0 } from "@auth0/auth0-react"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"

export default function Layout({ children }: { children: ReactNode }) {
  const { pathname } = useLocation()
  const { user, logout } = useAuth0()

  return (
    <div className="min-h-screen bg-background">
      <header className="border-b">
        <div className="container flex h-14 items-center gap-6">
          <Link to="/" className="flex items-center gap-2 font-semibold">
            <ShoppingBag className="h-5 w-5" />
            Shop
          </Link>
          <nav className="flex items-center gap-4 text-sm">
            <Link
              to="/catalog"
              className={cn(
                "text-muted-foreground hover:text-foreground transition-colors",
                pathname.startsWith("/catalog") && "text-foreground font-medium"
              )}
            >
              Catalog
            </Link>
            <Link
              to="/orders"
              className={cn(
                "text-muted-foreground hover:text-foreground transition-colors",
                pathname.startsWith("/orders") && "text-foreground font-medium"
              )}
            >
              Orders
            </Link>
          </nav>

          <div className="ml-auto flex items-center gap-3">
            <span className="text-sm text-muted-foreground">
              {user?.name ?? user?.email}
            </span>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
            >
              <LogOut className="mr-2 h-4 w-4" />
              Logout
            </Button>
          </div>
        </div>
      </header>
      <main className="container py-8">{children}</main>
    </div>
  )
}
