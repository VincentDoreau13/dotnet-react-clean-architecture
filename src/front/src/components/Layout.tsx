import type { ReactNode } from "react"
import { Link, useLocation } from "react-router-dom"
import { ShoppingBag } from "lucide-react"
import { cn } from "@/lib/utils"

export default function Layout({ children }: { children: ReactNode }) {
  const { pathname } = useLocation()

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
          </nav>
        </div>
      </header>
      <main className="container py-8">{children}</main>
    </div>
  )
}
