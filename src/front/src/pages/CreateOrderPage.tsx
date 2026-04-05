import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { ArrowLeft, Plus, Trash2 } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import Layout from "@/components/Layout"
import { catalogApi, isApiError } from "@/api/catalog"
import { ordersApi } from "@/api/orders"
import type { CreateOrderItemCommand } from "@/types/order"

interface OrderLine {
  catalogItemId: number
  catalogItemName: string
  unitPrice: number
  quantity: number
}

export default function CreateOrderPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [lines, setLines] = useState<OrderLine[]>([])
  const [selectedItemId, setSelectedItemId] = useState<string>("")
  const [quantity, setQuantity] = useState<string>("1")

  const { data: catalogItems = [] } = useQuery({
    queryKey: ["catalog-items"],
    queryFn: catalogApi.getItems,
  })

  const { mutate, isPending, isError, error } = useMutation({
    mutationFn: ordersApi.createOrder,
    onSuccess: (createdOrder) => {
      queryClient.invalidateQueries({ queryKey: ["orders"] })
      queryClient.invalidateQueries({ queryKey: ["catalog-items"] })
      navigate(`/orders/${createdOrder.id}`)
    },
  })

  const apiProblem = isApiError(error) ? error.response?.data : undefined
  const apiValidations = apiProblem?.extensions?.validations
  const apiErrorMessage =
    apiProblem?.detail ?? apiProblem?.title ?? "Failed to create order. Please try again."

  const availableItems = catalogItems.filter(
    (catalogItem) =>
      catalogItem.availableStock > 0 &&
      !lines.some((line) => line.catalogItemId === catalogItem.id)
  )

  function addLine() {
    const itemId = Number(selectedItemId)
    const qty = Number(quantity)
    if (!itemId || !qty || qty <= 0) return

    const catalogItem = catalogItems.find((item) => item.id === itemId)
    if (!catalogItem) return
    if (qty > catalogItem.availableStock) return

    setLines((previousLines) => [
      ...previousLines,
      {
        catalogItemId: catalogItem.id,
        catalogItemName: catalogItem.name,
        unitPrice: catalogItem.price,
        quantity: qty,
      },
    ])
    setSelectedItemId("")
    setQuantity("1")
  }

  function removeLine(catalogItemId: number) {
    setLines((previousLines) => previousLines.filter((line) => line.catalogItemId !== catalogItemId))
  }

  function handleSubmit() {
    if (lines.length === 0) return
    const items: CreateOrderItemCommand[] = lines.map((line) => ({
      catalogItemId: line.catalogItemId,
      quantity: line.quantity,
    }))
    mutate({ items })
  }

  const totalPrice = lines.reduce(
    (sum, line) => sum + line.unitPrice * line.quantity,
    0
  )

  return (
    <Layout>
      <div className="mb-6">
        <Button variant="ghost" size="sm" onClick={() => navigate("/orders")}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back to orders
        </Button>
      </div>

      <div className="max-w-2xl">
        <div className="mb-6">
          <h1 className="text-2xl font-bold tracking-tight">New Order</h1>
          <p className="text-muted-foreground">Select products and quantities</p>
        </div>

        <div className="grid gap-4">
          <Card>
            <CardHeader>
              <CardTitle>Add products</CardTitle>
              <CardDescription>Pick a product and set the quantity</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex gap-2">
                <select
                  className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-sm transition-colors focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring"
                  value={selectedItemId}
                  onChange={(changeEvent) => setSelectedItemId(changeEvent.target.value)}
                >
                  <option value="">Select a product…</option>
                  {availableItems.map((catalogItem) => (
                    <option key={catalogItem.id} value={catalogItem.id}>
                      {catalogItem.name} — {catalogItem.price.toFixed(2)} € (stock: {catalogItem.availableStock})
                    </option>
                  ))}
                </select>
                <Input
                  type="number"
                  min="1"
                  step="1"
                  className="w-24"
                  value={quantity}
                  onChange={(changeEvent) => setQuantity(changeEvent.target.value)}
                />
                <Button
                  type="button"
                  variant="outline"
                  onClick={addLine}
                  disabled={!selectedItemId}
                >
                  <Plus className="h-4 w-4" />
                </Button>
              </div>
            </CardContent>
          </Card>

          {lines.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle>Order summary</CardTitle>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Product</TableHead>
                      <TableHead className="text-right">Qty</TableHead>
                      <TableHead className="text-right">Unit Price</TableHead>
                      <TableHead className="text-right">Subtotal</TableHead>
                      <TableHead className="w-10" />
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {lines.map((line) => (
                      <TableRow key={line.catalogItemId}>
                        <TableCell className="font-medium">{line.catalogItemName}</TableCell>
                        <TableCell className="text-right">{line.quantity}</TableCell>
                        <TableCell className="text-right font-mono">
                          {line.unitPrice.toFixed(2)} €
                        </TableCell>
                        <TableCell className="text-right font-mono">
                          {(line.unitPrice * line.quantity).toFixed(2)} €
                        </TableCell>
                        <TableCell>
                          <Button
                            type="button"
                            variant="ghost"
                            size="sm"
                            onClick={() => removeLine(line.catalogItemId)}
                          >
                            <Trash2 className="h-4 w-4 text-destructive" />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                    <TableRow>
                      <TableCell colSpan={3} className="text-right font-semibold">
                        Total
                      </TableCell>
                      <TableCell className="text-right font-mono font-bold">
                        {totalPrice.toFixed(2)} €
                      </TableCell>
                      <TableCell />
                    </TableRow>
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          )}

          {isError && (
            <div className="rounded-md bg-destructive/10 p-3 text-destructive text-sm space-y-1">
              <p>{apiErrorMessage}</p>
              {apiValidations && apiValidations.length > 0 && (
                <ul className="list-disc list-inside">
                  {apiValidations.map((validationMessage, validationIndex) => (
                    <li key={validationIndex}>{validationMessage}</li>
                  ))}
                </ul>
              )}
            </div>
          )}

          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => navigate("/orders")}>
              Cancel
            </Button>
            <Button
              type="button"
              disabled={lines.length === 0 || isPending}
              onClick={handleSubmit}
            >
              {isPending ? "Creating…" : "Place Order"}
            </Button>
          </div>
        </div>
      </div>
    </Layout>
  )
}
