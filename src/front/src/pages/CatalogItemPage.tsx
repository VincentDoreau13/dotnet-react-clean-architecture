import { useQuery } from "@tanstack/react-query"
import { useNavigate, useParams } from "react-router-dom"
import { ArrowLeft, Package } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import Layout from "@/components/Layout"
import { catalogApi } from "@/api/catalog"

export default function CatalogItemPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const itemId = Number(id)
  const isValidId = id !== undefined && Number.isInteger(itemId) && itemId > 0
  // Use null in the query key when the ID is invalid so the key is stable
  // across renders (NaN !== NaN causes a new cache entry every render).
  const stableId = isValidId ? itemId : null

  const { data: item, isLoading, isError } = useQuery({
    queryKey: ["catalog-item", stableId],
    queryFn: () => catalogApi.getItemById(itemId),
    enabled: isValidId,
  })

  return (
    <Layout>
      <div className="mb-6">
        <Button variant="ghost" size="sm" onClick={() => navigate("/catalog")}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back to catalog
        </Button>
      </div>

      {!isValidId && (
        <div className="rounded-md bg-destructive/10 p-4 text-destructive text-sm">
          Invalid item ID.
        </div>
      )}

      {isValidId && isLoading && (
        <div className="flex items-center justify-center py-24 text-muted-foreground">
          Loading…
        </div>
      )}

      {isValidId && isError && (
        <div className="rounded-md bg-destructive/10 p-4 text-destructive text-sm">
          Item not found or the API is unreachable.
        </div>
      )}

      {item && (
        <div className="max-w-2xl">
          <div className="mb-6 flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-muted">
              <Package className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-2xl font-bold tracking-tight">{item.name}</h1>
              <p className="text-sm text-muted-foreground">Item #{item.id}</p>
            </div>
          </div>

          <div className="grid gap-4">
            <Card>
              <CardHeader>
                <CardTitle>Details</CardTitle>
                <CardDescription>Product information</CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <p className="text-sm font-medium text-muted-foreground mb-1">Description</p>
                  <p className="text-sm">{item.description}</p>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm font-medium text-muted-foreground mb-1">Price</p>
                    <p className="text-2xl font-bold font-mono">{item.price.toFixed(2)} €</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground mb-1">Stock</p>
                    <div className="flex items-center gap-2">
                      <p className="text-2xl font-bold">{item.availableStock}</p>
                      <Badge variant={item.availableStock > 0 ? "default" : "destructive"}>
                        {item.availableStock > 0 ? "In stock" : "Out of stock"}
                      </Badge>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Metadata</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-sm space-y-1">
                  <p className="text-muted-foreground">Created at</p>
                  <p>{new Date(item.createdAt).toLocaleString()}</p>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      )}
    </Layout>
  )
}
