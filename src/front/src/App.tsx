import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"
import CatalogPage from "@/pages/CatalogPage"
import CatalogItemPage from "@/pages/CatalogItemPage"
import CreateItemPage from "@/pages/CreateItemPage"
import OrdersPage from "@/pages/OrdersPage"
import OrderPage from "@/pages/OrderPage"
import CreateOrderPage from "@/pages/CreateOrderPage"

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/catalog" replace />} />
        <Route path="/catalog" element={<CatalogPage />} />
        {/* /catalog/new MUST stay above /catalog/:id — static segments win in RR v6 ranked matching */}
        <Route path="/catalog/new" element={<CreateItemPage />} />
        <Route path="/catalog/:id" element={<CatalogItemPage />} />
        <Route path="/orders" element={<OrdersPage />} />
        {/* /orders/new MUST stay above /orders/:id — same reason */}
        <Route path="/orders/new" element={<CreateOrderPage />} />
        <Route path="/orders/:id" element={<OrderPage />} />
      </Routes>
    </BrowserRouter>
  )
}
