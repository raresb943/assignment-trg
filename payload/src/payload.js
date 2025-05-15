const puppeteer = require("puppeteer");
const express = require("express");
const app = express();

app.use(express.json());

const port = process.env.PORT || 3000;

app.post("/browse", async (req, res) => {
  try {
    const { url } = req.body;

    if (!url) {
      return res.status(400).json({ error: "URL is required" });
    }

    console.log(`Browsing URL: ${url}`);

    const browser = await puppeteer.launch({
      headless: true,
      args: ["--no-sandbox", "--disable-setuid-sandbox"],
    });

    const page = await browser.newPage();
    await page.goto(url, { waitUntil: "networkidle2" });

    const html = await page.content();

    await browser.close();

    console.log(`Successfully retrieved HTML from ${url}`);

    res.json({ html });
  } catch (error) {
    console.error("Error browsing URL:", error);
    res.status(500).json({ error: error.message });
  }
});

app.get("/health", (req, res) => {
  res.json({ status: "ok" });
});

app.listen(port, () => {
  console.log(`Payload service listening on port ${port}`);
});
