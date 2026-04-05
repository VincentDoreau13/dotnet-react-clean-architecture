import { useQuery } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { Plus, ShoppingCart } from "lucide-react"
import { Button } from "@/components/ui/button"
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

export default function OrdersPage() {
  const navigate = useNavigate()
  const { data: orders, isLoading, isError } = useQuery({
    queryKey: ["orders"],
    queryFn: ordersApi.getOrders,
  })

  return (
    <Layout>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">Orders</h1>
          <p className="text-muted-foreground">Manage your orders</p>
        </div>
        <Button onClick={() => navigate("/orders/new")}>
          <Plus className="mr-2 h-4 w-4" />
          New Order
        </Button>
      </div>

      {isLoading && (
        <div className="flex items-center justify-center py-24 text-muted-foreground">
          Loading…
        </div>
      )}

      {isError && (
        <div className="rounded-md bg-destructive/10 p-4 text-destructive text-sm">
          Failed to load orders. Make sure the API is running.
        </div>
      )}

      {orders && (
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Order #</TableHead>
                <TableHead className="text-right">Items</TableHead>
                <TableHead className="text-right">Total</TableHead>
                <TableHead className="hidden sm:table-cell text-right">Created</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {orders.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-16 text-muted-foreground">
                    <ShoppingCart className="mx-auto mb-2 h-8 w-8 opacity-30" />
                    No orders yet. Create your first one.
                  </TableCell>
                </TableRow>
              ) : (
                orders.map((order) => (
                  <TableRow
                    key={order.id}
                    className="cursor-pointer"
                    role="button"
                    tabIndex={0}
                    onClick={() => navigate(`/orders/${order.id}`)}
                    onKeyDown={(keyDownEvent) => {
                      if (keyDownEvent.key === "Enter" || keyDownEvent.key === " ") {
                        keyDownEvent.preventDefault()
                        navigate(`/orders/${order.id}`)
                      }
                    }}
                    aria-label={`View order #${order.id}`}
                  >
                    <TableCell className="font-medium">#{order.id}</TableCell>
                    <TableCell className="text-right">
                      {order.items.length} product{order.items.length > 1 ? "s" : ""}
                    </TableCell>
                    <TableCell className="text-right font-mono">
                      {order.totalPrice.toFixed(2)} €
                    </TableCell>
                    <TableCell className="hidden sm:table-cell text-right text-muted-foreground text-sm">
                      {new Date(order.createdAt).toLocaleDateString()}
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
