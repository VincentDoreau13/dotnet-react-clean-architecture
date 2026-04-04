import { useNavigate } from "react-router-dom"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
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

const schema = z.object({
  name: z.string().min(1, "Name is required").max(200, "Max 200 characters"),
  description: z.string().min(1, "Description is required").max(1000, "Max 1000 characters"),
  price: z.coerce.number().positive("Price must be greater than 0"),
  availableStock: z.coerce.number().int().min(0, "Stock cannot be negative"),
})

type FormValues = z.infer<typeof schema>

export default function CreateItemPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()

  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { name: "", description: "", price: undefined, availableStock: 0 },
  })

  const { mutate, isPending, isError, error } = useMutation({
    mutationFn: catalogApi.createItem,
    onSuccess: (createdItem) => {
      queryClient.invalidateQueries({ queryKey: ["catalog-items"] })
      navigate(`/catalog/${createdItem.id}`)
    },
  })

  const apiProblem = isApiError(error) ? error.response?.data : undefined
  const apiValidations = apiProblem?.extensions?.validations
  const apiErrorMessage =
    apiProblem?.detail ?? apiProblem?.title ?? "Failed to create item. Please try again."

  return (
    <Layout>
      <div className="mb-6">
        <Button variant="ghost" size="sm" onClick={() => navigate("/catalog")}>
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back to catalog
        </Button>
      </div>

      <div className="max-w-xl">
        <div className="mb-6">
          <h1 className="text-2xl font-bold tracking-tight">New Catalog Item</h1>
          <p className="text-muted-foreground">Add a new product to the catalog</p>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Item details</CardTitle>
            <CardDescription>Fill in the product information below</CardDescription>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                onSubmit={form.handleSubmit((values) => mutate(values))}
                className="space-y-4"
              >
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Name</FormLabel>
                      <FormControl>
                        <Input placeholder="Product name" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="description"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Description</FormLabel>
                      <FormControl>
                        <Textarea placeholder="Describe the product…" rows={3} {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="price"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Price (€)</FormLabel>
                        <FormControl>
                          <Input type="number" step="0.01" min="0.01" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="availableStock"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Available Stock</FormLabel>
                        <FormControl>
                          <Input type="number" step="1" min="0" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

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

                <div className="flex justify-end gap-2 pt-2">
                  <Button type="button" variant="outline" onClick={() => navigate("/catalog")}>
                    Cancel
                  </Button>
                  <Button type="submit" disabled={isPending}>
                    {isPending ? "Creating…" : "Create Item"}
                  </Button>
                </div>
              </form>
            </Form>
          </CardContent>
        </Card>
      </div>
    </Layout>
  )
}
