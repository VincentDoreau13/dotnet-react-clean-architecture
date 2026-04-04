import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"
import CatalogPage from "@/pages/CatalogPage"
import CatalogItemPage from "@/pages/CatalogItemPage"
import CreateItemPage from "@/pages/CreateItemPage"

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/catalog" replace />} />
        <Route path="/catalog" element={<CatalogPage />} />
        {/* /catalog/new MUST stay above /catalog/:id — static segments win in RR v6 ranked matching */}
        <Route path="/catalog/new" element={<CreateItemPage />} />
        <Route path="/catalog/:id" element={<CatalogItemPage />} />
      </Routes>
    </BrowserRouter>
  )
}
