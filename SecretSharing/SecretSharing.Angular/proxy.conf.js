module.exports = {
  "/api/users/**": {
    target:
      process.env["services__userapi__https__0"] ||
      process.env["services__userapi__http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": "",
    },
  },
  "/api/secrets/*": {
    target:
      process.env["services__secretsapi__https__0"] ||
      process.env["services__secretsapi__http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": "",
    },
  },
  "/api/secrets*": {
    target:
      process.env["services__secretsapi__https__0"] ||
      process.env["services__secretsapi__http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": "",
    },
  },
};
