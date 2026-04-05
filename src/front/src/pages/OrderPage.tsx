import { useQuery } from "@tanstack/react-query"
import { useNavigate, useParams } from "react-router-dom"
import { ArrowLeft, ShoppingCart } from "lucide-react"
import { Button } from "@/components/ui/button"
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
import { ordersApi } from "@/api/orders"

export default function OrderPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const orderId = Number(id)
  const isValidId = id !== undefined && Number.isInteger(orderId) && orderId > 0
  const stableId = isValidId ? orderId : null

  const { data: order, isLoading, isError } = useQuery({
    queryKey: ["order", stableId],
    queryFn: () => ordersApi.getOrderById(orderId),
    enabled: isValidId,
  })

  return (
    <Layout>
      <div className="mb-6">
        <Button variant="ghost" size="sm" onClick={() => navigate("/orders")}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back to orders
        </Button>
      </div>

      {!isValidId && (
        <div className="rounded-md bg-destructive/10 p-4 text-destructive text-sm">
          Invalid order ID.
        </div>
      )}

      {isValidId && isLoading && (
        <div className="flex items-center justify-center py-24 text-muted-foreground">
          Loading…
        </div>
      )}

      {isValidId && isError && (
        <div className="rounded-md bg-destructive/10 p-4 text-destructive text-sm">
          Order not found or the API is unreachable.
        </div>
      )}

      {order && (
        <div className="max-w-2xl">
          <div className="mb-6 flex items-center gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-muted">
              <ShoppingCart className="h-5 w-5" />
            </div>
            <div>
              <h1 className="text-2xl font-bold tracking-tight">Order #{order.id}</h1>
              <p className="text-sm text-muted-foreground">
                Placed on {new Date(order.createdAt).toLocaleString()}
              </p>
            </div>
          </div>

          <div className="grid gap-4">
            <Card>
              <CardHeader>
                <CardTitle>Items</CardTitle>
                <CardDescription>
                  {order.items.length} product{order.items.length > 1 ? "s" : ""} in this order
                </CardDescription>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Product</TableHead>
                      <TableHead className="text-right">Qty</TableHead>
                      <TableHead className="text-right">Unit Price</TableHead>
                      <TableHead className="text-right">Subtotal</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {order.items.map((orderItem) => (
                      <TableRow key={orderItem.id}>
                        <TableCell className="font-medium">{orderItem.catalogItemName}</TableCell>
                        <TableCell className="text-right">{orderItem.quantity}</TableCell>
                        <TableCell className="text-right font-mono">
                          {orderItem.unitPrice.toFixed(2)} €
                        </TableCell>
                        <TableCell className="text-right font-mono">
                          {(orderItem.unitPrice * orderItem.quantity).toFixed(2)} €
                        </TableCell>
                      </TableRow>
                    ))}
                    <TableRow>
                      <TableCell colSpan={3} className="text-right font-semibold">
                        Total
                      </TableCell>
                      <TableCell className="text-right font-mono font-bold">
                        {order.totalPrice.toFixed(2)} €
                      </TableCell>
                    </TableRow>
                  </TableBody>
                </Table>
              </CardContent>
            </Card>

            <Card>
              <CardHeader>
                <CardTitle>Metadata</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="text-sm space-y-1">
                  <p className="text-muted-foreground">Created at</p>
                  <p>{new Date(order.createdAt).toLocaleString()}</p>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      )}
    </Layout>
  )
}
