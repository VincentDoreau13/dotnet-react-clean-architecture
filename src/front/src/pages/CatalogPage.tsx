import { useQuery } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { Plus, Package } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import Layout from "@/components/Layout"
import { catalogApi } from "@/api/catalog"

export default function CatalogPage() {
  const navigate = useNavigate()
  const { data: items, isLoading, isError } = useQuery({
    queryKey: ["catalog", "list"],
    queryFn: catalogApi.getItems,
  })

  return (
    <Layout>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Catalog</h1>
          <p className="text-muted-foreground">Manage your product catalog</p>
        </div>
        <Button onClick={() => navigate("/catalog/new")}>
          <Plus className="mr-2 h-4 w-4" />
          New Item
        </Button>
      </div>

      {isLoading && (
        <div className="flex items-center justify-center py-24 text-muted-foreground">
          Loading…
        </div>
      )}

      {isError && (
        <div className="rounded-md bg-destructive/10 p-4 text-destructive text-sm">
          Failed to load catalog items. Make sure the API is running.
        </div>
      )}

      {items && (
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead className="hidden md:table-cell">Description</TableHead>
                <TableHead className="text-right">Price</TableHead>
                <TableHead className="text-right">Stock</TableHead>
                <TableHead className="hidden sm:table-cell text-right">Created</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={5} className="text-center py-16 text-muted-foreground">
                    <Package className="mx-auto mb-2 h-8 w-8 opacity-30" />
                    No items yet. Create your first one.
                  </TableCell>
                </TableRow>
              ) : (
                items.map((catalogItem) => (
                  <TableRow
                    key={catalogItem.id}
                    className="cursor-pointer"
                    role="button"
                    tabIndex={0}
                    onClick={() => navigate(`/catalog/${catalogItem.id}`)}
                    onKeyDown={(keyDownEvent) => {
                      if (keyDownEvent.key === "Enter" || keyDownEvent.key === " ") {
                        keyDownEvent.preventDefault()
                        navigate(`/catalog/${catalogItem.id}`)
                      }
                    }}
                    aria-label={`View ${catalogItem.name}`}
                  >
                    <TableCell className="font-medium">{catalogItem.name}</TableCell>
                    <TableCell className="hidden md:table-cell text-muted-foreground max-w-xs truncate">
                      {catalogItem.description}
                    </TableCell>
                    <TableCell className="text-right font-mono">
                      {catalogItem.price.toFixed(2)} €
                    </TableCell>
                    <TableCell className="text-right">
                      <Badge variant={catalogItem.availableStock > 0 ? "default" : "destructive"}>
                        {catalogItem.availableStock}
                      </Badge>
                    </TableCell>
                    <TableCell className="hidden sm:table-cell text-right text-muted-foreground text-sm">
                      {new Date(catalogItem.createdAt).toLocaleDateString()}
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>
      )}
    </Layout>
  )
}
