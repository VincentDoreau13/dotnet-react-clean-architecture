import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { useEffect } from "react"
import { useNavigate, useParams } from "react-router-dom"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { ArrowLeft, Package } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import Layout from "@/components/Layout"
import { catalogApi, isApiError } from "@/api/catalog"

const updateStockSchema = z.object({
  availableStock: z.coerce
    .number()
    .int()
    .min(0, "Stock cannot be negative")
    .max(1_000_000, "Stock cannot exceed 1 000 000"),
})

type UpdateStockFormValues = z.infer<typeof updateStockSchema>

export default function CatalogItemPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const itemId = Number(id)
  const isValidId = id !== undefined && Number.isInteger(itemId) && itemId > 0
  // Use null in the query key when the ID is invalid so the key is stable
  // across renders (NaN !== NaN causes a new cache entry every render).
  const stableId = isValidId ? itemId : null

  const { data: item, isLoading, isError } = useQuery({
    queryKey: ["catalog", "detail", stableId],
    queryFn: () => {
      if (!isValidId) throw new Error("Invalid item ID")
      return catalogApi.getItemById(itemId)
    },
    enabled: isValidId,
  })

  const stockForm = useForm<UpdateStockFormValues>({
    resolver: zodResolver(updateStockSchema),
    values: { availableStock: item?.availableStock ?? 0 },
  })

  const {
    mutate: updateStock,
    reset: resetUpdateStock,
    isPending: isUpdateStockPending,
    isSuccess: isUpdateStockSuccess,
    isError: isUpdateStockError,
    error: updateStockError,
  } = useMutation({
    mutationFn: (formValues: UpdateStockFormValues) => {
      if (!isValidId) throw new Error("Invalid item ID")
      return catalogApi.updateItemStock(itemId, formValues)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["catalog", "detail", stableId] })
      queryClient.invalidateQueries({ queryKey: ["catalog", "list"] })
    },
  })

  useEffect(() => {
    resetUpdateStock()
  }, [item?.availableStock, resetUpdateStock])

  const stockApiProblem = isApiError(updateStockError) ? updateStockError.response : undefined
  const stockApiErrorMessage = (() => {
    if (!stockApiProblem) return "Failed to update stock. Please try again."
    if (stockApiProblem.status >= 500) return "A server error occurred. Please try again later."
    return stockApiProblem.data?.detail ?? stockApiProblem.data?.title ?? "Failed to update stock. Please try again."
  })()

  const handleBackNavigation = () => navigate("/catalog")

  return (
    <Layout>
      <div className="mb-6">
        <Button variant="ghost" size="sm" onClick={handleBackNavigation}>
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
                <CardTitle>Update Stock</CardTitle>
                <CardDescription>Adjust the available quantity for this item</CardDescription>
              </CardHeader>
              <CardContent>
                <Form {...stockForm}>
                  <form
                    onSubmit={stockForm.handleSubmit((formValues) => {
                      resetUpdateStock()
                      updateStock(formValues)
                    })}
                    className="flex items-end gap-3"
                  >
                    <FormField
                      control={stockForm.control}
                      name="availableStock"
                      render={({ field: availableStockField }) => (
                        <FormItem className="flex-1">
                          <FormLabel>Available Stock</FormLabel>
                          <FormControl>
                            <Input type="number" step="1" min="0" {...availableStockField} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                    <Button type="submit" disabled={isUpdateStockPending}>
                      {isUpdateStockPending ? "Saving…" : "Update stock"}
                    </Button>
                  </form>
                </Form>

                {isUpdateStockError && (
                  <div className="mt-3 rounded-md bg-destructive/10 p-3 text-destructive text-sm">
                    {stockApiErrorMessage}
                  </div>
                )}

                {isUpdateStockSuccess && (
                  <div className="mt-3 rounded-md bg-green-500/10 p-3 text-green-700 text-sm">
                    Stock updated successfully.
                  </div>
                )}
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Metadata</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-sm space-y-3">
                  <div>
                    <p className="text-muted-foreground">Created at</p>
                    <p>{new Date(item.createdAt).toLocaleString()}</p>
                  </div>
                  <div>
                    <p className="text-muted-foreground">Updated at</p>
                    <p>{new Date(item.updatedAt).toLocaleString()}</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      )}
    </Layout>
  )
}
